using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
        //[HttpPost]
        //public async Task<ActionResult<Service>> PosttblServices(Service tblServices)
        //{
        //    _context.tblServices.Add(tblServices);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GettblServices", new { id = tblServices.ServiceID }, tblServices);
        //}

        // POST: api/tblServices/CreateNewService
        [HttpPost("CreateNewService")]
        public async Task<IActionResult> CreateNewService(
            DateTime serviceDate,
            int transportTimeUsed,
            int transportKmUsed,
            int workTimeUsed,
            string serviceImage,
            int machineID,
            int customerID,
            string machineSerialNumber,
            string note,
            string machineStatus,
            int partId,
            int partsUsed
            )
        {
            // Call the stored procedure to create a new service
                await _context.tblServices.FromSqlRaw("EXECUTE dbo.CreateNewService @ServiceDate, @TransportTimeUsed, @TransportKmUsed, @WorkTimeUsed, @ServiceImage, @MachineID, @CustomerID, @MachineSerialNumber, @Note, @MachineStatus, @PartID, @PartsUsed",
                new SqlParameter("@ServiceDate", serviceDate),
                new SqlParameter("@TransportTimeUsed", transportTimeUsed),
                new SqlParameter("@TransportKmUsed", transportKmUsed),
                new SqlParameter("@WorkTimeUsed", workTimeUsed),
                new SqlParameter("@ServiceImage", serviceImage),
                new SqlParameter("@MachineID", machineID),
                new SqlParameter("@CustomerID", customerID),
                new SqlParameter("@MachineSerialNumber", machineSerialNumber),
                new SqlParameter("@Note", note),
                new SqlParameter("@MachineStatus", machineStatus),
                new SqlParameter("@PartID", partId),
                new SqlParameter("@PartsUsed", partsUsed)).ToListAsync();

            return Ok();
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

    [HttpGet("CalculateInvoice/{serviceID}")]
    public async Task<IActionResult> CalculateInvoice(int serviceID)
    {
        // Call the stored procedure to calculate the invoice for the given service ID
        var invoice = await _context.InvoiceViewModel
            .FromSqlRaw("EXECUTE dbo.CalculateInvoice @serviceid",
                new SqlParameter("@serviceid", serviceID))
            .FirstOrDefaultAsync();

        if (invoice == null)
        {
            return NotFound();
        }

        return Ok(invoice);
    }
}
