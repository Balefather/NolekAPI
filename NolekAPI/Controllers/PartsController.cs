using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NolekAPI.Data;
using NolekAPI.Model;

namespace NolekAPI.Controllers
{
    [EnableCors("AllowAllOrigins")]
    [Route("api/[controller]")]
    [ApiController]
    public class PartsController : ControllerBase
    {

        private readonly NolekAPIContext _context;

        public PartsController(NolekAPIContext context)
        {
            _context = context;
        }

        // GET: api/Parts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Part>>> GetPart()
        {
            return await _context.tblParts.ToListAsync();
        }

        [HttpGet("ByCustomerID")]
        public async Task<ActionResult<List<Machine>>> GetPartByCustomerID(int ID)
        {
            CustomersController cmpController = new(_context);
            Customer cmp = cmpController.GetCustomerByID(ID).Result.Value;
            List<Machine> ms = new();
            foreach (var m in cmp.Machines)
            {
                ms.Add(m);
            }
            return ms;
        }
        // GET: api/Parts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Part>> GettblParts(int id)
        {
            var tblParts = await _context.tblParts.FindAsync(id);

            if (tblParts == null)
            {
                return NotFound();
            }

            return tblParts;
        }

        [HttpGet("Search/{term}")]
        public async Task<ActionResult<IEnumerable<Part>>> Search(string term)
        {
            return await _context.tblParts.Where(tblParts => tblParts.PartName.Contains(term)).ToListAsync();
        }

        // PUT: api/Parts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PuttblParts(int id, Part tblParts)
        {
            if (id != tblParts.PartID)
            {
                return BadRequest();
            }

            _context.Entry(tblParts).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!tblPartsExists(id))
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

        // POST: api/Parts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [EnableCors("AllowAllOrigins")]
        [HttpPost]
        public async Task<ActionResult<Part>> PosttblParts(Part tblParts)
        {
            _context.tblParts.Add(tblParts);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GettblParts", new { id = tblParts.PartID }, tblParts);
        }

        // DELETE: api/Parts/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeletetblParts(int id)
        //{
        //    var tblParts = await _context.tblParts.FindAsync(id);
        //    if (tblParts == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.tblParts.Remove(tblParts);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        // DELETE: api/Parts/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletetblParts(int id)
        {
            try
            {
                // Call the stored procedure to delete the Part and its relations
                await _context.Database.ExecuteSqlRawAsync("EXECUTE dbo.DeletePart @PartID = {0}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        private bool tblPartsExists(int id)
        {
            return _context.tblParts.Any(e => e.PartID == id);
        }
    }
}
