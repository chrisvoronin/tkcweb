using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TKC.Data;
using TKC.Models;

namespace TKC.Controllers
{

    [ApiController]
    [Route("api/resources")]
    public class ApiResourcesController : Controller
    {
        private readonly CacheService _cache;
        private readonly ApplicationDbContext _context;
        private readonly YoutubeAPI _api;
        private readonly int pageSize = 10;

        public ApiResourcesController(CacheService cache, YoutubeAPI api, ApplicationDbContext context)
        {
            _cache = cache;
            _context = context;
            _api = api;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Read(int id)
        {
            try
            {
                var model = await _context.Resources.FirstOrDefaultAsync(st => st.Id == id);

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
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {

            try
            {
                var toDelete = _context.Resources.Find(id);

                if (toDelete != null)
                {
                    _context.Resources.Remove(toDelete);
                    await _context.SaveChangesAsync();
                }

                return NoContent();
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

            var file = formData.Files["file"];
            string? url = null;
            string? title = null;
            long groupId = 0;

            if (formData.ContainsKey("url"))
            {
                url = formData["url"].ToString();
            }

            if (formData.ContainsKey("name"))
            {
                title = formData["name"].ToString();
            }

            if (formData.ContainsKey("group"))
            {
                string group = formData["group"].ToString();
                _ = long.TryParse(group, out groupId);
            }

            bool hasEitherFileOrUrl = file != null || !string.IsNullOrWhiteSpace(url);

            if (string.IsNullOrWhiteSpace(title)
                || groupId == 0
                || !hasEitherFileOrUrl)
            {
                return BadRequest("Missing information needed to create.");
            }

            // now build and save
            string? fileName = null;

            if (file != null)
            {
                fileName = await FileUtility.SaveDocument(file);
            }
             
            try
            {
                var st = new ResourceItem();
                st.GroupId = groupId;
                st.FileName = fileName ?? url ?? "";
                st.Text = title;

                _context.Resources.Add(st);
                await _context.SaveChangesAsync();

                return Json(st);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(long id, [FromForm] IFormCollection formData)
        {
            var file = formData.Files["file"];
            string? url = null;

            string? title = null;
            long groupId = 0;

            if (formData.ContainsKey("url"))
            {
                url = formData["url"].ToString();
            }

            if (formData.ContainsKey("name"))
            {
                title = formData["name"].ToString();
            }

            if (formData.ContainsKey("group"))
            {
                string group = formData["group"].ToString();
                _ = long.TryParse(group, out groupId);
            }

            // now build and save
            string? fileName = null;

            if (file != null)
            {
                fileName = await FileUtility.SaveDocument(file);
            } else if (!string.IsNullOrWhiteSpace(url))
            {
                fileName = url;
            }

            try
            {
                var st = _context.Resources.Find(id);

                if (st == null)
                {
                    return NotFound();
                }

                st.GroupId = groupId;
                st.Text = title ?? st.Text;
                st.FileName = fileName ?? st.FileName;
                
                await _context.SaveChangesAsync();

                return Json(st);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

    }
}

