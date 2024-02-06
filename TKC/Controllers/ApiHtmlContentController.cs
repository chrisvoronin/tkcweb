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
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(long id, [FromForm] IFormCollection formData)
        {
            string? title = null;
            string? html = null;

            if (formData.ContainsKey("title"))
            {
                title = formData["title"].ToString();
            }
            if (formData.ContainsKey("html"))
            {
                html = formData["html"].ToString();
            }

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(html))
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

                existing.Name = title;
                existing.Value = html;

                _cache.Remove("Liturgy");

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        private static bool IsHtmlSafe(string htmlContent)
        {
            // Check for external links
            if (ContainsUnsafeLink(htmlContent, "<a[^>]+?href\\s*=\\s*(['\"])(.*?)\\1"))
            {
                Console.WriteLine("Unsafe: Contains external links");
                return false;
            }

            // Check for external images
            if (ContainsUnsafeTag(htmlContent, "<img[^>]+?src\\s*=\\s*(['\"])(.*?)\\1"))
            {
                Console.WriteLine("Unsafe: Contains external images");
                return false;
            }

            // Check for script tags
            if (ContainsUnsafeTag(htmlContent, "<script[^>]*>.*?</script>"))
            {
                Console.WriteLine("Unsafe: Contains script tags");
                return false;
            }

            // If none of the unsafe tags were found, the HTML is safe
            return true;
        }

        private static bool ContainsUnsafeLink(string htmlContent, string pattern)
        {
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(htmlContent);
            foreach (Match match in matches)
            {
                string url = match.Groups[2].Value;
                if (!IsLocalUrl(url))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool ContainsUnsafeTag(string htmlContent, string pattern)
        {
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(htmlContent);
        }

        private static bool IsLocalUrl(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
            {
                return uri.Host.Contains("thekingscongregation.com", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}

