using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class CustomersMachinesPartsController : ControllerBase
    {
        private readonly NolekAPIContext _context;

        public CustomersMachinesPartsController(NolekAPIContext context)
        {
            _context = context;
        }

        // GET: api/CustomersMachinesParts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomersMachinesParts>>> GetCustomersMachinesParts()
        {
            //if (_context.vw_CustomersMachinesParts == null)
            //{
            //    return NotFound();
            //}
            //  return await _context.vw_CustomersMachinesParts.ToListAsync();
            using (_context)
            {
                var query = @"SELECT * FROM vw_CustomersMachinesParts";

                return  _context.vw_CustomersMachinesParts.FromSqlRaw(query).ToList();

            }
        }
        [Route("Customers")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            if (_context.vw_CustomersMachinesParts == null)
            {
                return NotFound();
            }
            return await ToCustomers(await _context.vw_CustomersMachinesParts.ToListAsync());
        }

        [HttpGet("customers/search/{term}")]
        public async Task<ActionResult<IEnumerable<Customer>>> Search(string term)
        {
            return await ToCustomers(await _context.vw_CustomersMachinesParts.Where(customer => customer.CustomerName.Contains(term)).ToListAsync());
        }

        // GET: api/CustomersMachinesParts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomersMachinesParts>> GetCustomersMachinesParts(int id)
        {
          if (_context.vw_CustomersMachinesParts == null)
          {
              return NotFound();
          }
            var customersMachinesParts = await _context.vw_CustomersMachinesParts.FindAsync(id);

            if (customersMachinesParts == null)
            {
                return NotFound();
            }

            return customersMachinesParts;
        }

        // PUT: api/CustomersMachinesParts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomersMachinesParts(int id, CustomersMachinesParts customersMachinesParts)
        {
            if (id != customersMachinesParts.CustomerID)
            {
                return BadRequest();
            }

            _context.Entry(customersMachinesParts).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomersMachinesPartsExists(id))
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

        // POST: api/CustomersMachinesParts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CustomersMachinesParts>> PostCustomersMachinesParts(CustomersMachinesParts customersMachinesParts)
        {
          if (_context.vw_CustomersMachinesParts == null)
          {
              return Problem("Entity set 'NolekAPIContext.CustomersMachinesParts'  is null.");
          }
            _context.vw_CustomersMachinesParts.Add(customersMachinesParts);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomersMachinesParts", new { id = customersMachinesParts.CustomerID }, customersMachinesParts);
        }

        // DELETE: api/CustomersMachinesParts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomersMachinesParts(int id)
        {
            if (_context.vw_CustomersMachinesParts == null)
            {
                return NotFound();
            }
            var customersMachinesParts = await _context.vw_CustomersMachinesParts.FindAsync(id);
            if (customersMachinesParts == null)
            {
                return NotFound();
            }

            _context.vw_CustomersMachinesParts.Remove(customersMachinesParts);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomersMachinesPartsExists(int id)
        {
            return (_context.vw_CustomersMachinesParts?.Any(e => e.CustomerID == id)).GetValueOrDefault();
        }

        private async Task<ActionResult<IEnumerable<NolekAPI.Model.Customer>>> ToCustomers(IEnumerable<CustomersMachinesParts> CustomersMachinesPartsList)
        {

            List<Customer> customersMachinesParts2List = new List<Customer>();

            foreach (var customerGroup in CustomersMachinesPartsList.GroupBy(x => x.CustomerID))
            {
                var customer = customerGroup.First();

                var machines = customerGroup.GroupBy(x => x.MachineSerialNumber)
                                            .Select(machineGroup =>
                                            {
                                                var machine = machineGroup.First();
                                                var parts = machineGroup.Select(x => new Part2
                                                {
                                                    PartID = x.PartID,
                                                    PartName = x.PartName,
                                                    NumberInStock = x.NumberInStock,
                                                    PartPrice = x.PartPrice,
                                                    AmountPartMachine = x.AmountPartMachine,
                                                }).ToList();
                                                return new Machine
                                                {
                                                    MachineID= machine.MachineID,
                                                    MachineName = machine.MachineName,
                                                    PartsMustChange = machine.PartsMustChange,
                                                    ServiceInterval = machine.ServiceInterval,
                                                    MachineSerialNumber = machine.MachineSerialNumber,
                                                    Parts = parts
                                                };
                                            }).ToList();

                var customer2 = new Customer
                {
                    CustomerID = customer.CustomerID,
                    CustomerName = customer.CustomerName,
                    CustomerAddress = customer.CustomerAddress,
                    PhoneNumber = customer.PhoneNumber,
                    Email = customer.Email,
                    Machines = machines
                };

                customersMachinesParts2List.Add(customer2);
            }

            return customersMachinesParts2List;
        }
    }
}
