using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.PortableExecutable;
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
        public async Task<ActionResult<IEnumerable<ServiceViewGrouped>>> GettblServices()
        {
            //var tblServices = await _context.vw_Services.FromSqlRaw("EXECUTE dbo.sp_GetAllServices").ToListAsync();
            var tblServices = await _context.vw_Services.ToListAsync();

            if (tblServices == null || tblServices.Count == 0)
            {
                return NotFound();
            }
            var groupServices = await FromServiceViewToService(tblServices);
            return groupServices;

        }

        private async Task<ActionResult<IEnumerable<ServiceViewGrouped>>> FromServiceViewToService(IEnumerable<ServiceView> serviceViewList)
        {
            List<NolekAPI.Model.ServiceViewGrouped> groupServices = new List<NolekAPI.Model.ServiceViewGrouped>();

            foreach (var serviceGroup in serviceViewList.GroupBy(x => x.ServiceID))
            {
                var service = serviceGroup.First();
                var parts = serviceGroup.Select(x => new ServicePart2
                {
                    PartID = x.PartID,
                    PartName = x.PartName ?? "",
                    PartsUsed = x.PartsUsed
                }).Distinct<ServicePart2>().ToList();

                var images = serviceGroup.Select(y => new Image
                {
                    ImagePath = y.ImagePath
                }).Distinct<Image>().ToList();

                groupServices.Add(new NolekAPI.Model.ServiceViewGrouped
                {
                    ServiceID = service.ServiceID,
                    ServiceDate = service.ServiceDate,
                    CustomerName = service.CustomerName,
                    MachineName = service.MachineName,
                    MachineSerialNumber = service.MachineSerialNumber,
                    TransportTimeUsed = service.TransportTimeUsed,
                    TransportKmUsed = service.TransportKmUsed,
                    WorkTimeUsed = service.WorkTimeUsed,
                    Note = service.Note,
                    MachineStatus = service.MachineStatus,
                    Parts = parts,
                    Images = images
                }) ;
            }
            return groupServices;
        }


        [HttpGet("ByDate")]
        public async Task<ActionResult<IEnumerable<ServiceView>>> GetServicesByDate()
        {
            var tblServices = await _context.vw_Services.FromSqlRaw("EXECUTE dbo.sp_GetAllServices").ToListAsync();
            var customers = await _context.vw_CustomersMachinesParts.ToListAsync();

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
        public async Task<IActionResult> CreateNewService(Service service)
        {
            DateTime serviceDate = DateTime.UtcNow;
            // Call the stored procedure to create a new service and get the ServiceID
            var serviceId = await _context.tblServices.FromSqlRaw("EXECUTE dbo.CreateNewService @ServiceDate, @TransportTimeUsed, @TransportKmUsed, @WorkTimeUsed, @MachineID, @CustomerID, @MachineSerialNumber, @Note, @MachineStatus, @PartID, @PartsUsed",
                new SqlParameter("@ServiceDate", serviceDate),
                new SqlParameter("@TransportTimeUsed", service.TransportTimeUsed),
                new SqlParameter("@TransportKmUsed", service.TransportKmUsed),
                new SqlParameter("@WorkTimeUsed", service.WorkTimeUsed),
                new SqlParameter("@MachineID", service.MachineID),
                new SqlParameter("@CustomerID", service.CustomerID),
                new SqlParameter("@MachineSerialNumber", service.MachineSerialNumber),
                new SqlParameter("@Note", service.Note),
                new SqlParameter("@MachineStatus", service.MachineStatus),
                new SqlParameter("@PartID", service.ServiceParts[0].PartID),
                new SqlParameter("@PartsUsed", service.ServiceParts[0].PartsUsed))
                .Select(x => x.ServiceID)
                .FirstOrDefaultAsync();

            // Create a new Service object with the returned ServiceID
            var createdService = new Service { ServiceID = serviceId };

            foreach (var servicePart in service.ServiceParts)
            {
                servicePart.ServiceID = createdService.ServiceID;
                _context.tblServices_Parts.Add(servicePart);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(CreateNewService), new { id = createdService.ServiceID }, createdService.ServiceID);
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

        [HttpGet("CalculateInvoice/{serviceID}")]
        public async Task<IActionResult> CalculateInvoice(int serviceID)
        {
            // Call the stored procedure to calculate the invoice for the given service ID
            var invoice = await _context.Invoice.FromSqlRaw("EXECUTE dbo.CalculateInvoice @serviceid",
                new SqlParameter("@serviceid", serviceID)).ToListAsync();

            if (invoice == null)
            {
                return NotFound();
            }

            return Ok(invoice);
        }
    }
}
