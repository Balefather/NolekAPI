using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NolekAPI.Data;
using NolekAPI.Model;
using NolekAPI.Model.Dto;
using NolekAPI.Model.Dto.Junction;
using NolekAPI.Model.View;

namespace NolekAPI.Controllers
{
    [EnableCors("AllowAllOrigins")]
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
        public async Task<ActionResult<IEnumerable<Service>>> GetServices()
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

        private async Task<ActionResult<IEnumerable<NolekAPI.Model.Service>>> FromServiceViewToService(IEnumerable<ServiceView> serviceViewList)
        {
            List<Service> groupServices = new();

            foreach (var serviceGroup in serviceViewList.GroupBy(x => x.ServiceID))
            {
                var service = serviceGroup.First();
                var parts = serviceGroup.Select(x => new ServicePart
                {
                    PartID = x.PartID,
                    PartName = x.PartName ?? "",
                    PartsUsed = x.PartsUsed
                }).Distinct<ServicePart>().ToList();

                var images = serviceGroup.Select(y => new ImageDto
                {
                    ImagePath = y.ImagePath
                }).Distinct<ImageDto>().ToList();

                groupServices.Add(new NolekAPI.Model.Service
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
            var tblServices = GetServices().Result.Value.ToList().First(x => x.ServiceID == id);

            if (tblServices == null)
            {
                return NotFound();
            }
            return tblServices;
        }

        // GET: api/tblServices/5
        [HttpGet("ImagesByServiceID/{id}")]
        public async Task<ActionResult<List<ImageDto>>> GetServiceImages(int id)
        {
            var serviceImages = _context.tblServices_Images.Where(x => x.ServiceID == id).ToList();
            var images = _context.tblImages.ToList();

            List<ImageDto> result = new List<ImageDto>();
            foreach (var serviceImage in serviceImages)
            {
                foreach (var image in images)
                {
                    if (image.ImageID == serviceImage.ImageID)
                    {
                        result.Add(image);
                    }
                }


            }


            if (result == null)
            {
                return NotFound();
            }
            return result;
        }

        // PUT: api/tblServices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PuttblServices(int id, ServiceDto tblServices)
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
        public async Task<IActionResult> CreateNewService(ServiceDto service)
        {

            service.ServiceDate = DateTime.UtcNow;
            _context.tblServices.Add(service);
            await _context.SaveChangesAsync();

            ObjectResult createdServiceResult = CreatedAtAction("CreateNewService", new { id = service.ServiceID }, service);
            ServiceDto createdService = (ServiceDto)createdServiceResult.Value;

            if (createdService != null)
            {
                if (service.ServiceParts == null || service.ServiceParts.Count == 0)
                {
                    var sp = new ServicePartJunctionDto()
                    {
                        ServiceID = 0,
                        PartID = _context.tblParts.First().PartID,
                        PartsUsed = 999
                    };
                }
                foreach (var servicePart in service.ServiceParts)
                {
                    servicePart.ServiceID = createdService.ServiceID;
                    _context.tblServices_Parts.Add(servicePart);
                    await _context.SaveChangesAsync();
                }
            }

            if (createdServiceResult != null)
            {
                MachinesController mc = new(_context);
                //DateTime now = DateTime.UtcNow;
                //DateTime nextService = now.AddMonths((mc.GetCustomerMachine(createdService.MachineSerialNumber).Result.Value.ServiceInterval));
                CustomerMachineJunctionDto cmj = new CustomerMachineJunctionDto() {
                    MachineSerialNumber= createdService.MachineSerialNumber,
                    CustomerID= createdService.CustomerID,
                    NextService = DateTime.UtcNow.AddMonths((mc.GetCustomerMachine(createdService.MachineSerialNumber).Result.Value.ServiceInterval)),

                };
                mc.SetNextServiceDate(createdService.MachineSerialNumber, cmj);
                //return Ok();
                return CreatedAtAction("GettblServices", new { id = service.ServiceID }, service);
            }
            return NotFound();
        }

        // DELETE: api/tblServices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletetblServices(int id)
        {
            var service = await _context.tblServices.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            //remove serviceparts

            var serviceParts = _context.tblServices_Parts.Where(x => x.ServiceID == id).ToList();
            foreach (var item in serviceParts)
            {
                _context.tblServices_Parts.Remove(item);
                await _context.SaveChangesAsync();
            }

            //remove serviceimages

            var serviceImages = _context.tblServices_Images.Where(x => x.ServiceID == id).ToList();
            foreach (var item in serviceImages)
            {
                _context.tblServices_Images.Remove(item);
                await _context.SaveChangesAsync();
            }

            //finally, remove the service
            _context.tblServices.Remove(service);
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
