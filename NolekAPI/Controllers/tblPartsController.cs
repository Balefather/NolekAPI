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
    public class tblPartsController : ControllerBase
    {
        private readonly NolekAPIContext _context;

        public tblPartsController(NolekAPIContext context)
        {
            _context = context;
        }

        // GET: api/tblParts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<tblParts>>> GetPart()
        {
            return await _context.tblParts.ToListAsync();
        }

        // GET: api/tblParts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<tblParts>> GettblParts(int id)
        {
            var tblParts = await _context.tblParts.FindAsync(id);

            if (tblParts == null)
            {
                return NotFound();
            }

            return tblParts;
        }

        // PUT: api/tblParts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PuttblParts(int id, tblParts tblParts)
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

        // POST: api/tblParts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<tblParts>> PosttblParts(tblParts tblParts)
        {
            _context.tblParts.Add(tblParts);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GettblParts", new { id = tblParts.PartID }, tblParts);
        }

        // DELETE: api/tblParts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletetblParts(int id)
        {
            var tblParts = await _context.tblParts.FindAsync(id);
            if (tblParts == null)
            {
                return NotFound();
            }

            _context.tblParts.Remove(tblParts);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool tblPartsExists(int id)
        {
            return _context.tblParts.Any(e => e.PartID == id);
        }
    }
}
