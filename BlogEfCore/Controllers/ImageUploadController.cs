using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StellarAdmin.Serialization.Models;

namespace BlogEfCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public ImageUploadController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            if (file.Length == 0)
            {
                return BadRequest();
            }

            var extension = Path.GetExtension(file.FileName);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var storagePath = Path.Combine(_environment.WebRootPath, "uploads", "images", fileName);

            await using var fs = new FileStream(storagePath, FileMode.Create);
            await file.CopyToAsync(fs);

            return Ok(new UploadResult(fileName));
        }
    }
}