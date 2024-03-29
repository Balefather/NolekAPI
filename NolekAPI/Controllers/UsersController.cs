﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NolekAPI.Data;
using NolekAPI.Model;
using NolekAPI.Model.Dto;
using NolekAPI.Model.Dto.Junction;
using NuGet.Packaging.Signing;

namespace NolekAPI.Controllers
{
    [EnableCors("AllowAllOrigins")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly NolekAPIContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(NolekAPIContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        //[HttpGet("restricted2")]
        //[Authorize(Roles = "Admin")]
        //public IActionResult GetRestrictedData2()
        //{
        //    // Retrieve the user's identity from the JWT token
        //    var identity = HttpContext.User.Identity as ClaimsIdentity;
        //    if (identity == null)
        //    {
        //        return Unauthorized();
        //    }

        //    // Retrieve the user's role from the JWT token's claims
        //    var roleClaim = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        //    if (roleClaim == null)
        //    {
        //        return Unauthorized();
        //    }

        //    // Check if the user is authorized to access the restricted data
        //    if (roleClaim.Value != "Admin")
        //    {
        //        return Forbid();
        //    }

        //    // Return the restricted data
        //    return Ok("This is restricted data that only Admin users can access.");
        //}

        //[HttpGet("restricted")]
        //[Authorize]
        //public IActionResult GetRestrictedData()
        //{
        //    // Retrieve the user's identity from the JWT token
        //    var identity = HttpContext.User.Identity as ClaimsIdentity;
        //    if (identity == null)
        //    {
        //        return Unauthorized();
        //    }

        //    // Retrieve the user's role from the JWT token's claims
        //    var roleClaim = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        //    if (roleClaim == null)
        //    {
        //        return Unauthorized();
        //    }

        //    // Check if the user is authorized to access the restricted data
        //    if (roleClaim.Value != "Admin")
        //    {
        //        return Forbid();
        //    }

        //    // Return the restricted data
        //    return Ok("This is restricted data that only Admin users can access.");
        //}

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {

            var userCredentials = _context.tblUsers.FirstOrDefault(uc => uc.Username == loginModel.Username);
            var roleNames = GetRolesByUserId(userCredentials.UserID);

            if (userCredentials == null || userCredentials.Password != loginModel.Password)
            {
                return Unauthorized();
            }
            else
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, loginModel.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };


                foreach (var roleName in roleNames)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName));
            }

            var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims.ToArray(),
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: credentials
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
        }

        [HttpGet("GetRolesByID")]
        [Authorize(Roles = "Superadministrator")]
        public List<String> GetRolesByUserId(int userID)
        {
            List<String> roleNames = new List<String>();
            if (_context.tblUserRoles == null)
            {
                //probably log this failed, somehow.
                return roleNames;
            }
            List<UserRoleJunctionDto> userRoles = _context.tblUserRoles.Where(u => u.UserID.Equals(userID)).ToList();

            if (!userRoles.Any())
            {
                //probably log this failed, somehow.
                return roleNames;
            }

            foreach (var userRole in userRoles)
            {
                roleNames.Add(_context.tblRoles.Find(userRole.RoleID).RoleName ?? "No role associated with roleId. This should never happen");
            }
            return roleNames;
        }

        
        private async Task<ActionResult<int>> GetRoleIdFromName(string rolename)
        {
            if (_context.tblRoles == null)
            {
                return NotFound();
            }
            var role = await _context.tblRoles.FindAsync(rolename);

            if (role == null)
            {
                return NotFound();
            }

            return role.RoleID;
        }

        // DELETE: api/Users/5
        [HttpDelete("RemoveRoleFromUser")]
        [Authorize(Roles = "Superadministrator")]
        public async Task<IActionResult> RemoveRoleFromUser(string rolename, int userid)
        {

            var role = await _context.tblRoles.FirstOrDefaultAsync(r => r.RoleName == rolename);
            if (role == null)
            {
                return NotFound();
            }

            var userRole = await _context.tblUserRoles.FirstOrDefaultAsync(ur => ur.UserID == userid && ur.RoleID == role.RoleID);
            if (userRole == null)
            {
                return NotFound();
            }

            _context.tblUserRoles.Remove(userRole);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Users/5
        [HttpPost("AddRoleToUser")]
        [Authorize(Roles = "Superadministrator")]
        public async Task<IActionResult> AddRoleToUser(string rolename, int userid)
        {

            var role = await _context.tblRoles.FirstOrDefaultAsync(r => r.RoleName == rolename);
            if (role == null)
            {
                return NotFound();
            }

            //var userRole = await _context.tblUserRoles.FirstOrDefaultAsync(ur => ur.UserID == userid && ur.RoleID == role.RoleID);
            //if (userRole == null)
            //{
            //    return NotFound();
            //}

            UserRoleJunctionDto userRole = new() { RoleID = role.RoleID, UserID = userid };
            if (!_context.tblUserRoles.Contains(userRole))
            {
                _context.tblUserRoles.Add(userRole);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }



        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            if (_context.tblUsers == null)
            {
                return NotFound();
            }
            return await _context.tblUsers.ToListAsync();
        }
        // GET: api/Users
        [HttpGet("Roles")]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            if (_context.tblRoles == null)
            {
                return NotFound();
            }
            return await _context.tblRoles.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            if (_context.tblUsers == null)
            {
                return NotFound();
            }
            var user = await _context.tblUsers.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UserDto user)
        {
            if (id != user.UserID)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserDto>> PostUser(UserDto user)
        {
            if (_context.tblUsers == null)
            {
                return Problem("Entity set 'NolekAPIContext.User'  is null.");
            }
            _context.tblUsers.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserID }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (_context.tblUsers == null)
            {
                return NotFound();
            }
            var user = await _context.tblUsers.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.tblUsers.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return (_context.tblUsers?.Any(e => e.UserID == id)).GetValueOrDefault();
        }
    }
}
