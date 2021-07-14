using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StellarAdmin.Editors.Models;
using StellarAdmin.Serialization.Models;

namespace BlogEfCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadsController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public UploadsController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost("file")]
        public async Task<IActionResult> UploadFile([FromForm] ImageEditorUploadRequest request)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest();
            }

            var extension = Path.GetExtension(request.File.FileName);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var storagePath = Path.Combine(_environment.WebRootPath, "uploads", "images", fileName);

            await using var fs = new FileStream(storagePath, FileMode.Create);
            await request.File.CopyToAsync(fs);

            return Ok(new ImageEditorUploadResponse(fileName));
        }
        
        [HttpPost("mde-image")]
        public async Task<IActionResult> UploadMarkdownEditorImage([FromForm] MarkdownEditorImageUploadRequest request)
        {
            if (request.Image == null || request.Image.Length == 0)
            {
                return BadRequest();
            }

            var extension = Path.GetExtension(request.Image.FileName);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var storagePath = Path.Combine(_environment.WebRootPath, "uploads", "images", fileName);

            await using var fs = new FileStream(storagePath, FileMode.Create);
            await request.Image.CopyToAsync(fs);

            return Ok(new MarkdownEditorImageUploadResponse($"{Request.Scheme}://{Request.Host}{Request.PathBase}/uploads/images/{fileName}"));
        }

    }
}