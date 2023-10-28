using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TKC.Data;
using TKC.Models;

namespace TKC.Controllers
{

    [ApiController]
    [Route("api/shorttake")]
    public class ApiShortTakesController : Controller
    {
        private readonly CacheService _cache;
        private readonly ApplicationDbContext _context;
        private readonly YoutubeAPI _api;
        private readonly int pageSize = 10;

        public ApiShortTakesController(CacheService cache, YoutubeAPI api, ApplicationDbContext context)
        {
            _cache = cache;
            _context = context;
            _api = api;
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
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

        [Authorize]
        [HttpPost()]
        public async Task<IActionResult> Create([FromBody] ShortTake st)
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

        [Authorize]
        [HttpPatch()]
        public async Task<IActionResult> Update([FromBody] ShortTake st)
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
                existing.SubTitle = st.SubTitle;
                existing.Author = st.Author;
                existing.VideoUrl = st.VideoUrl;
                existing.PdfUrl = st.PdfUrl;
                existing.AudioUrl = st.AudioUrl;
                existing.DateCreated = st.DateCreated;

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{id}")]
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


        [HttpGet("page/{page}")]
        public async Task<IActionResult> Page(int page)
        {
            if (page <= 0)
            {
                return BadRequest();
            }

            int skip = (page - 1) * pageSize;

            ShortTakeResponse resp = new ShortTakeResponse
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalResults = await _context.ShortTakes.CountAsync(),
                Items = await _context.ShortTakes.Skip(skip).Take(pageSize).ToListAsync()
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
                var query = _context.ShortTakes.Where(st => st.Title.ToLower().Contains(lowerQ) || st.SubTitle.ToLower().Contains(lowerQ) || st.Author.ToLower().Contains(lowerQ));

                ShortTakeResponse resp = new ShortTakeResponse
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

    }
}

