using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TKC.Data;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TKC.Controllers
{

    [ApiController]
    [Route("api/htmlcontent")]
    public class ApiHtmlContentController : Controller
    {
        private readonly CacheService _cache;
        private readonly ApplicationDbContext _context;

        public ApiHtmlContentController(CacheService cache, ApplicationDbContext context)
        {
            _cache = cache;
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Read(int id)
        {
            try
            {
                var model = await _context.HTMLContents.FirstOrDefaultAsync(st => st.Id == id);

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
        [HttpPost]
        public async Task<string> UploadImage([FromForm] IFormFile file)
        {
            string fileName = await FileUtility.SaveFile(file);
            return "/Uploads/" + fileName;
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] IFormCollection formData)
        {
            string? html = null;

            if (formData.ContainsKey("html"))
            {
                html = formData["html"].ToString();
            }

            if (string.IsNullOrWhiteSpace(html))
            {
                return StatusCode(400, "Title and HTML cannot be blank.");
            }

            if (!IsHtmlSafe(html))
            {
                return StatusCode(400, "HTML cannot have scripts or external links.");
            }
            
            try
            {
                var existing = _context.HTMLContents.Find(id);

                if (existing == null)
                {
                    return NotFound();
                }

                if (formData.ContainsKey("title"))
                {
                    existing.Name = formData["title"].ToString();
                }

                existing.Value = html;

                await _context.SaveChangesAsync();

                _cache.Remove(existing.Name);
                _cache.Set(existing.Name, existing.Value);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        private static bool IsHtmlSafe(string htmlContent)
        {            
            // Check for script tags
            if (ContainsUnsafeTag(htmlContent, "<script[^>]*>.*?</script>"))
            {
                Console.WriteLine("Unsafe: Contains script tags");
                return false;
            }

            // If none of the unsafe tags were found, the HTML is safe
            return true;
        }

        private static bool ContainsUnsafeTag(string htmlContent, string pattern)
        {
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(htmlContent);
        }

        
    }
}

