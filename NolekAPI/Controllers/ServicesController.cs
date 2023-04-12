using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NolekAPI.Data;
using NolekAPI.Model;

namespace NolekAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly NolekAPIContext _context;

        public ServicesController(NolekAPIContext context)
        {
            _context = context;
        }

        // GET: api/tblServices
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Service>>> GettblServices()
        //{
        //    return await _context.tblServices.ToListAsync();
        //}

        // GET: api/Services
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Service>>> GettblServices()
        {
            var tblServices = await _context.tblServices.FromSqlRaw("EXECUTE dbo.sp_GetAllServices").ToListAsync();

            if (tblServices == null || tblServices.Count == 0)
            {
                return NotFound();
            }

            return tblServices;

        }


        // GET: api/tblServices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Service>> GettblServices(int id)
        {
            var tblServices = await _context.tblServices.FindAsync(id);

            if (tblServices == null)
            {
                return NotFound();
            }

            return tblServices;
        }

        // PUT: api/tblServices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PuttblServices(int id, Service tblServices)
        {
            if (id != tblServices.ServiceID)
            {
                return BadRequest();
            }

            _context.Entry(tblServices).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!tblServicesExists(id))
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

        // POST: api/tblServices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Service>> PosttblServices(Service tblServices)
        {
            _context.tblServices.Add(tblServices);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GettblServices", new { id = tblServices.ServiceID }, tblServices);
        }

        // DELETE: api/tblServices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletetblServices(int id)
        {
            var tblServices = await _context.tblServices.FindAsync(id);
            if (tblServices == null)
            {
                return NotFound();
            }

            _context.tblServices.Remove(tblServices);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool tblServicesExists(int id)
        {
            return _context.tblServices.Any(e => e.ServiceID == id);
        }
    }
}
