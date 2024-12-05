using System.Drawing.Printing;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TKC.Data;
using TKC.Models;


namespace TKC.Controllers
{

    [ApiController]
    [Route("api/blog")]
    public class ApiBlogController : Controller
    {
        private readonly int pageSize = 10;
        private readonly CacheService _cache;
        private readonly ApplicationDbContext _context;

        public ApiBlogController(CacheService cache, ApplicationDbContext context)
        {
            _cache = cache;
            _context = context;
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> Read(int id)
        {
            try
            {
                var model = await _context.Blogs.FirstOrDefaultAsync(st => st.id == id);

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

        [HttpGet("page/{page}")]
        public async Task<IActionResult> Page(int page, [FromQuery] int? topic, [FromQuery] int? status, [FromQuery] string? text, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            if (page <= 0)
            {
                return BadRequest();
            }

            int skip = (page - 1) * pageSize;

            // Start building the query
            var query = _context.Blogs.AsQueryable();

            // Add optional filters
            if (status.HasValue)
            {
                query = query.Where(b => b.status == status.Value);
            }

            if (topic.HasValue)
            {
                query = query.Where(b => b.categoryId == topic.Value);
            }

            if (!string.IsNullOrWhiteSpace(text))
            {
                query = query.Where(b => b.title.Contains(text));
            }

            if (fromDate.HasValue && toDate.HasValue)
            {
                query = query.Where(b => b.dateCreated >= fromDate.Value && b.dateCreated <= toDate.Value);
            } else
            {
                if (fromDate.HasValue)
                {
                    query = query.Where(b => b.dateCreated >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(b => b.dateCreated <= toDate.Value);
                }
            }

            // Calculate total results with the applied filters
            int totalResults = await query.CountAsync();

            // Fetch the paginated results
            var items = await query
                .OrderByDescending(i => i.id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            // Prepare the response
            PagedResponse<BlogPost> resp = new PagedResponse<BlogPost>
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalResults = totalResults,
                Items = items
            };

            // Truncate HTML content for each item
            foreach (var item in resp.Items)
            {
                item.html = GetPreviewText(item.html ?? "", 200);
            }

            return Json(resp);
        }


        [Authorize]
        [HttpPost()]
        public async Task<IActionResult> Create([FromForm] IFormCollection formData)
        {
            string? errorMessage = Validate(formData, out BlogPost result);
            if (errorMessage != null)
            {
                return StatusCode(400, errorMessage);
            }

            try
            {
                result.dateCreated = DateTime.Now;
                result.createdBy = User.Identity?.Name ?? "";
                _context.Blogs.Add(result);
                await _context.SaveChangesAsync();

                return Ok(result.id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }


        [Authorize]
        [HttpPost("image")]
        public async Task<string> UploadImage([FromForm] IFormFile file)
        {
            string fileName = await FileUtility.SaveFile(file);
            return "/File/" + fileName;
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var existing = _context.Blogs.Find(id);

                if (existing == null)
                {
                    return NotFound();
                }

                existing.status = 2;
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error while deleting blog");
            }
        }

        [Authorize]
        [HttpPut("{id}/publish")]
        public async Task<IActionResult> Publish(long id)
        {
            try
            {
                var existing = _context.Blogs.Find(id);

                if (existing == null)
                {
                    return NotFound();
                }

                existing.status = 1;
                await _context.SaveChangesAsync();

                return Ok(existing.status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error while deleting blog");
            }
        }

        [Authorize]
        [HttpPut("{id}/unpublish")]
        public async Task<IActionResult> UnPublish(long id)
        {
            try
            {
                var existing = _context.Blogs.Find(id);

                if (existing == null)
                {
                    return NotFound();
                }

                existing.status = 0;
                await _context.SaveChangesAsync();

                return Ok(existing.status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error while deleting blog");
            }
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(long id, [FromForm] IFormCollection formData)
        {
            string? errorMessage = Validate(formData, out BlogPost result);
            if (errorMessage != null)
            {
                return StatusCode(400, errorMessage);
            }

            try
            {
                var existing = _context.Blogs.Find(id);

                if (existing == null)
                {
                    return NotFound();
                }

                existing.title = result.title;
                existing.categoryId = result.categoryId;
                existing.headerImage = result.headerImage?.Replace("/File/", ""); // can be null
                existing.html = result.html;

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error while updating blog");
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

        private string? Validate(IFormCollection formData, out BlogPost result)
        {
            result = new BlogPost();

            // Extract data
            string? html = formData["html"];
            string? headerImage = formData["headerImage"]; // Can be null
            string? title = formData["title"];
            string? categoryText = formData["category"];

            // Initialize parsed values
            int categoryId = 0;

            // Validate inputs
            if (string.IsNullOrWhiteSpace(title))
                return "Title cannot be blank.";

            if (string.IsNullOrWhiteSpace(categoryText) || !int.TryParse(categoryText, out categoryId))
                return "Category must be a valid numeric value.";

            if (string.IsNullOrWhiteSpace(html))
                return "HTML content cannot be blank.";

            if (!IsHtmlSafe(html))
                return "HTML cannot have scripts or external links.";

            // Construct result object
            result = new BlogPost
            {
                title = title,
                html = html,
                headerImage = headerImage,
                categoryId = categoryId,
                status = 0
            };

            return null;
        }

        private string GetPreviewText(string htmlContent, int characterLimit = 200)
        {
            if (string.IsNullOrEmpty(htmlContent))
                return string.Empty;

            // Load the HTML into HtmlDocument
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            // Extract the plain text
            var plainText = htmlDoc.DocumentNode.InnerText;

            // Truncate to the specified character limit
            if (plainText.Length > characterLimit)
                plainText = plainText.Substring(0, characterLimit) + "...";

            return plainText;
        }

    }
}

