using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TKC.Data;
using TKC.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TKC.Controllers
{

    [Authorize]
    [Route("[controller]")]
    public class ApiController : Controller
    {
        private readonly CacheService _cache;
        private readonly ApplicationDbContext _context;
        private readonly YoutubeAPI _api;

        public ApiController(CacheService cache, YoutubeAPI api, ApplicationDbContext context)
        {
            _cache = cache;
            _context = context;
            _api = api;
        }

        // GET: /<controller>/
        // get
        [HttpGet("shorttakes/{id}")]
        public async Task<IActionResult> Read(int id)
        {
            try
            {
                var shortTake = await _context.ShortTakes.FirstOrDefaultAsync(st => st.Id == id);

                if (shortTake == null)
                {
                    return NotFound();
                }

                return Json(shortTake);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("shorttakes/{id}")]
        public async Task<IActionResult> Delete(int id)
        {

            try
            {
                var toDelete = _context.ShortTakes.Find(id);

                if (toDelete != null)
                {
                    _context.ShortTakes.Remove(toDelete);
                    await _context.SaveChangesAsync();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("shorttakes")]
        public async Task<IActionResult> Create(ShortTake st)
        {
            if (st == null)
            {
                return BadRequest("The ShortTake data is null.");
            }

            try
            {
                _context.ShortTakes.Add(st);
                await _context.SaveChangesAsync();

                return Json(st);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPatch("shorttakes")]
        public async Task<IActionResult> Update(ShortTake st)
        {
            if (st == null)
            {
                return BadRequest("ShortTake data is null.");
            }

            try
            {
                var existing = _context.ShortTakes.Find(st.Id);

                if (existing == null)
                {
                    return NotFound();
                }

                existing.Title = st.Title;
                existing.Speaker = st.Speaker;
                existing.Url = st.Url;

                await _context.SaveChangesAsync();

                return Json(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Music




        // Generic

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                // Check if the file is null
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Please select a file.");
                }

                // Define a unique file name
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                // Define the path to save the file
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName);

                // Create the directory if it doesn't exist
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                // Save the file to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // You can save additional information about the file to a database or perform other actions here

                return Ok(new { FileName = fileName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

