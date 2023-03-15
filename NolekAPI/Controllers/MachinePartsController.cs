﻿using System;
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
    public class MachinePartsController : ControllerBase
    {
        private readonly NolekAPIContext _context;

        public MachinePartsController(NolekAPIContext context)
        {
            _context = context;
        }

        // GET: api/MachineParts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MachineParts>>> GetMachineParts()
        {
          if (_context.vw_MachineParts == null)
          {
              return NotFound();
          }
            return await _context.vw_MachineParts.ToListAsync();
        }

        // GET: api/MachineParts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MachineParts>> GetMachineParts(int id)
        {
          if (_context.vw_MachineParts == null)
          {
              return NotFound();
          }
            var machineParts = await _context.vw_MachineParts.FindAsync(id);

            if (machineParts == null)
            {
                return NotFound();
            }

            return machineParts;
        }

        // PUT: api/MachineParts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMachineParts(int id, MachineParts machineParts)
        {
            if (id != machineParts.MachineID)
            {
                return BadRequest();
            }

            _context.Entry(machineParts).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MachinePartsExists(id))
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

        // POST: api/MachineParts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MachineParts>> PostMachineParts(MachineParts machineParts)
        {
          if (_context.vw_MachineParts == null)
          {
              return Problem("Entity set 'NolekAPIContext.MachineParts'  is null.");
          }
            _context.vw_MachineParts.Add(machineParts);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMachineParts", new { id = machineParts.MachineID }, machineParts);
        }

        // DELETE: api/MachineParts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMachineParts(int id)
        {
            if (_context.vw_MachineParts == null)
            {
                return NotFound();
            }
            var machineParts = await _context.vw_MachineParts.FindAsync(id);
            if (machineParts == null)
            {
                return NotFound();
            }

            _context.vw_MachineParts.Remove(machineParts);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MachinePartsExists(int id)
        {
            return (_context.vw_MachineParts?.Any(e => e.MachineID == id)).GetValueOrDefault();
        }
    }
}
