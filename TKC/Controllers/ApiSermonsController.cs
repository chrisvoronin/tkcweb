using System;
using System.Collections.Generic;
using System.Drawing.Printing;
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

    [ApiController]
    [Route("api/sermon")]
    public class ApiSermonsController : Controller
    {
        private readonly CacheService _cache;
        private readonly ApplicationDbContext _context;
        private readonly YoutubeAPI _api;
        private readonly int pageSize = 10;

        public ApiSermonsController(CacheService cache, YoutubeAPI api, ApplicationDbContext context)
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
                var model = await _context.Sermons.FirstOrDefaultAsync(st => st.Id == id);

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
                var toDelete = _context.Sermons.Find(id);

                if (toDelete != null)
                {
                    _context.Sermons.Remove(toDelete);
                    await _context.SaveChangesAsync();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("page/{page}")]
        public async Task<IActionResult> Page(int page)
        {
            if (page <= 0)
            {
                return BadRequest();
            }

            int skip = (page - 1) * pageSize;

            SermonsResponse resp = new SermonsResponse
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalResults = await _context.Sermons.CountAsync(),
                Items = await _context.Sermons
                .OrderByDescending(i => i.DateCreated)
                .Skip(skip).Take(pageSize).ToListAsync()
            };

            return Json(resp);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 3)
            {
                return BadRequest();
            }

            try
            {
                var lowerQ = q.ToLower();
                var query = _context.Sermons.Where(st => st.Title.ToLower().Contains(lowerQ) || st.SubTitle.ToLower().Contains(lowerQ) || st.Author.ToLower().Contains(lowerQ));

                SermonsResponse resp = new SermonsResponse
                {
                    Items = await query.Take(pageSize).ToListAsync(),
                    CurrentPage = 1,
                    ItemsPerPage = pageSize,
                    TotalResults = await query.CountAsync()
                };

                return Json(resp);
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

            var audio = formData.Files["audio"];
            var pdf = formData.Files["pdf"];

            if (audio == null)
            {
                return BadRequest("Audio file is required.");
            }

            string? title = null;
            string? subTitle = null;
            string? author = null;
            string? videoUrl = null;
            DateTime? dateCreated = null;

            if (formData.ContainsKey("title"))
            {
                title = formData["title"].ToString();
            }
            else
            {
                return BadRequest("Title is required.");
            }

            if (formData.ContainsKey("subTitle"))
            {
                subTitle = formData["subTitle"].ToString();
            }

            if (formData.ContainsKey("author"))
            {
                author = formData["author"].ToString();
            }
            else
            {
                return BadRequest("Speaker is required.");
            }
            if (formData.ContainsKey("videoUrl"))
            {
                videoUrl = formData["videoUrl"].ToString();
            }
            if (formData.ContainsKey("dateCreated"))
            {
                var dateCreatedString = formData["dateCreated"];
                if (DateTime.TryParse(dateCreatedString, out var parsedDate))
                {
                    dateCreated = parsedDate;
                }
                else
                {
                    return BadRequest("Created Date is required.");
                }
            }
            else
            {
                return BadRequest("Created Date is required.");
            }

            // now build and save
            string? audioFileName = null;
            string? pdfFileName = null;

            if (audio != null)
            {
                audioFileName = await FileUtility.SaveFile(audio);
            }

            if (pdf != null)
            {
                pdfFileName = await FileUtility.SaveFile(pdf);
            }

            try
            {
                var st = new Sermon();

                st.Title = title;
                st.SubTitle = subTitle;
                st.Author = author;
                st.VideoUrl = videoUrl;
                st.DateCreated = dateCreated ?? DateTime.Now;

                if (audioFileName != null)
                {
                    st.AudioUrl = audioFileName;
                }

                if (pdfFileName != null)
                {
                    st.PdfUrl = pdfFileName;
                }

                _context.Sermons.Add(st);
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

            var audio = formData.Files["audio"];
            var pdf = formData.Files["pdf"];

            string? title = null;
            string? subTitle = null;
            string? author = null;
            string? videoUrl = null;
            DateTime? dateCreated = null;

            if (formData.ContainsKey("title"))
            {
                title = formData["title"].ToString();
            }
            if (formData.ContainsKey("subTitle"))
            {
                subTitle = formData["subTitle"].ToString();
            }
            if (formData.ContainsKey("author"))
            {
                author = formData["author"].ToString();
            }
            if (formData.ContainsKey("videoUrl"))
            {
                videoUrl = formData["videoUrl"].ToString();
            }
            if (formData.ContainsKey("dateCreated"))
            {
                var dateCreatedString = formData["dateCreated"];
                if (DateTime.TryParse(dateCreatedString, out var parsedDate))
                {
                    dateCreated = parsedDate;
                }
            }

            // Now handle filling out form
            string? audioFileName = null;
            string? pdfFileName = null;

            if (audio != null)
            {
                audioFileName = await FileUtility.SaveFile(audio);
            }

            if (pdf != null)
            {
                pdfFileName = await FileUtility.SaveFile(pdf);
            }

            try
            {
                var existing = _context.Sermons.Find(id);

                if (existing == null)
                {
                    return NotFound();
                }

                if (title != null)
                    existing.Title = title;

                if (subTitle != null)
                    existing.SubTitle = subTitle;

                if (author != null)
                    existing.Author = author;

                if (videoUrl != null)
                    existing.VideoUrl = videoUrl;

                if (audioFileName != null)
                    existing.AudioUrl = audioFileName;

                if (pdfFileName != null)
                    existing.PdfUrl = pdfFileName;

                if (dateCreated != null)
                    existing.DateCreated = dateCreated.Value;

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

    }
}

