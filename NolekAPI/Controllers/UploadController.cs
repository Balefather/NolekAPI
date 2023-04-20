using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NolekAPI.Data;
using NolekAPI.Model.Dto;
using NolekAPI.Model.Dto.Junction;

namespace NolekAPI.Controllers
{
    [EnableCors("AllowAllOrigins")]
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly NolekAPIContext _context;

        public UploadController(NolekAPIContext context)
        {
            _context = context;
        }

  
        // POST: api/upload
        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            // Implement file upload logic here
            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine("uploads", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Ok(new { message = "File uploaded successfully" });
        }

        [HttpPost("Service")]
        public async Task<IActionResult> UploadFileService([FromForm] IFormFile file, int serviceID)
        {
            // Implement file upload logic here
            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine("uploads", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            ImageDto newImage= new ImageDto() { 
            ImagePath = filePath
            };

            _context.tblImages.Add(newImage);
            _context.SaveChanges();

            ObjectResult createdImageResult = CreatedAtAction("UploadFileService", new { id = newImage.ImageID }, newImage);
            ImageDto createdImage = (ImageDto)createdImageResult.Value;

            ServiceImageJunctionDto si = new()
            {
                ServiceID = serviceID,
                ImageID = createdImage.ImageID
            };

            _context.tblServices_Images.Add(si);
            _context.SaveChanges();

            return Ok(new { message = "File uploaded successfully" });
        }


    }
}
