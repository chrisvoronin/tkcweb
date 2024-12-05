using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TKC.Data;
using TKC.Models;


namespace TKC.Controllers
{

    [ApiController]
    [Route("api/blogtopic")]
    public class ApiBlogTopicController : Controller
    {
        private readonly CacheService _cache;
        private readonly ApplicationDbContext _context;

        public ApiBlogTopicController(CacheService cache, ApplicationDbContext context)
        {
            _cache = cache;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var model = await _context.BlogCategories.ToListAsync();
                return Json(model);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> Read(int id)
        {
            try
            {
                var model = await _context.BlogCategories.FirstOrDefaultAsync(st => st.id == id);

                if (model == null)
                {
                    return NotFound();
                }

                return Json(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [Authorize]
        [HttpPost()]
        public async Task<IActionResult> Create([FromForm] IFormCollection formData)
        {
            
            string? name = formData["name"];
            string? flags = formData["flags"];

            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name is required");
            }

            if (string.IsNullOrEmpty(flags))
            {
                return BadRequest("Flag content is required");
            }

            if (!int.TryParse(flags, out int flagInt))
            {
                return BadRequest("Flag content is invalid");
            }

            try
            {
                BlogCategory content = new BlogCategory();
                content.name = name;
                content.flags = flagInt;

                _context.BlogCategories.Add(content);
                await _context.SaveChangesAsync();

                return Json(content);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] IFormCollection formData)
        {
            string? name = formData["name"];
            string? flags = formData["flags"];

            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name is required");
            }

            if (string.IsNullOrEmpty(flags))
            {
                return BadRequest("Flag content is required");
            }

            if (!int.TryParse(flags, out int flagInt))
            {
                return BadRequest("Flag content is invalid");
            }

            try
            {
                var existing = _context.BlogCategories.Find(id);

                if (existing == null)
                {
                    return NotFound();
                }

                existing.name = name;
                existing.flags = flagInt;

                await _context.SaveChangesAsync();

                return Json(existing);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }
    }
}

