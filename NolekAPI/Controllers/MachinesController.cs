using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NolekAPI.Data;
using NolekAPI.Model;
using NolekAPI.Model.Dto;
using NolekAPI.Model.Dto.Junction;
using NolekAPI.Model.View;

namespace NolekAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MachinesController : ControllerBase
    {
        private readonly NolekAPIContext _context;

        public MachinesController(NolekAPIContext context)
        {
            _context = context;
        }

        // GET: api/MachineParts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NolekAPI.Model.Machine>>> GetMachines()
        {
            if (_context.vw_MachineParts == null)
            {
                return NotFound();
            }
            return await ToMachines(await _context.vw_MachineParts.ToListAsync());
        }

        // GET: api/MachineParts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NolekAPI.Model.Machine>> GetMachine(int id)
        {
            if (_context.vw_MachineParts == null)
            {
                return NotFound();
            }
            var machineParts = await _context.vw_MachineParts.Where(machine => machine.MachineID == id).ToListAsync(); 

            if (machineParts == null)
            {
                return NotFound();
            }
            var machinesActionResult = await ToMachines(machineParts);
            var machines = machinesActionResult.Value;

            return machines.First();
        }

        // GET: api/MachineParts/5
        [HttpGet("CustomerMachine/{id}")]
        public async Task<ActionResult<NolekAPI.Model.CustomerMachine>> GetCustomerMachine(string serialNumber)
        {
            if (_context.vw_MachineParts == null)
            {
                return NotFound();
            }
            //Get customermachines
            var customerMachineJunction = _context.tblCustomers_Machines.Where(machine => machine.MachineSerialNumber == serialNumber).First();
            // Join customermachines on serialnumber to get machineID
            var machineSerialNumbers = _context.tblMachineSerialNumbers.Where(m => m.MachineSerialNumber == serialNumber).First();
            // Get Machines
            var machine = _context.tblMachines.Where(machine => machine.MachineID == machineSerialNumbers.MachineID).First();

            CustomerMachine cm = new()
            {
                CustomerID = customerMachineJunction.CustomerID,
                MachineID = machine.MachineID,
                MachineName = machine.MachineName,
                PartsMustChange = machine.PartsMustChange,
                ServiceInterval = machine.ServiceInterval,
                MachineSerialNumber = serialNumber

            };


            return cm;
        }



        [HttpGet("search/{term}")]
        public async Task<ActionResult<IEnumerable<NolekAPI.Model.Machine>>> Search(string term)
        {
            return await ToMachines(await _context.vw_MachineParts.Where(machine => machine.MachineName.Contains(term)).ToListAsync());
        }

        // PUT: api/MachineParts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMachineParts(int id, MachineView machineParts)
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
        public async Task<ActionResult<MachineView>> PostMachineParts(MachineView machineParts)
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

        private bool CustomerMachineJunctionExists(string serialNumber)
        {
            return (_context.tblCustomers_Machines?.Any(e => e.MachineSerialNumber == serialNumber)).GetValueOrDefault();
        }

        private async Task<ActionResult<IEnumerable<NolekAPI.Model.Machine>>> ToMachines(IEnumerable<MachineView> machinesPartsList)
        {
            List<NolekAPI.Model.Machine> machinesParts2List = new List<NolekAPI.Model.Machine>();

            foreach (var machineGroup in machinesPartsList.GroupBy(x => x.MachineID))
            {
                var machine = machineGroup.First();
                var parts = machineGroup.Select(x => new MachinePart
                {
                    PartID = x.PartID,
                    PartName = x.PartName,
                    NumberInStock = x.NumberInStock,
                    PartPrice = x.PartPrice
                }).ToList();

                machinesParts2List.Add(new NolekAPI.Model.Machine
                {
                    MachineID= machine.MachineID,
                    MachineName = machine.MachineName,
                    PartsMustChange = machine.PartsMustChange,
                    ServiceInterval = machine.ServiceInterval,
                    Parts = parts
                });
            }
            return machinesParts2List;
        }

        [HttpGet("GoAway")]
        public async Task<IActionResult> SetNextServiceDate(string serialNumber, CustomerMachineJunctionDto cmj)
        {
            var existingEntity = await _context.tblCustomers_Machines.FindAsync(serialNumber);
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).State = EntityState.Detached;
            }
            _context.Entry(cmj).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerMachineJunctionExists(serialNumber))
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

        //private async Task<ActionResult<NolekAPI.Model.Machine>> ToMachine(MachineParts machinesPartsList)
        //{
        //    List<NolekAPI.Model.Machine> machinesParts2List = new List<NolekAPI.Model.Machine>();

        //    foreach (var machineGroup in machinesPartsList.GroupBy(x => x.MachineID))
        //    {
        //        var machine = machineGroup.First();
        //        var parts = machineGroup.Select(x => new Part2
        //        {
        //            PartID = x.PartID,
        //            PartName = x.PartName,
        //            NumberInStock = x.NumberInStock,
        //            PartPrice = x.PartPrice,
        //            AmountPartMachine = x.AmountPartMachine,
        //        }).ToList();

        //        machinesParts2List.Add(new NolekAPI.Model.Machine
        //        {
        //            MachineID = machine.MachineID,
        //            MachineName = machine.MachineName,
        //            //PartsMustChange = machine.PartsMustChange,
        //            //ServiceInterval = machine.ServiceInterval,
        //            Parts = parts
        //        });
        //    }
        //    return machinesParts2List;
        //}
    }
}
