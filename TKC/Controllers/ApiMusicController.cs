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

    [ApiController]
    [Route("api/music")]
    public class ApiMusicController : Controller
    {
        private readonly CacheService _cache;
        private readonly ApplicationDbContext _context;
        private readonly YoutubeAPI _api;
        private readonly int pageSize = 2;

        public ApiMusicController(CacheService cache, YoutubeAPI api, ApplicationDbContext context)
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
                var model = await _context.Musics.FirstOrDefaultAsync(st => st.Id == id);

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
                var toDelete = _context.Musics.Find(id);

                if (toDelete != null)
                {
                    _context.Musics.Remove(toDelete);
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
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Music st)
        {
            if (st == null)
            {
                return BadRequest("The Music data is null.");
            }

            try
            {
                _context.Musics.Add(st);
                await _context.SaveChangesAsync();

                return Json(st);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [Authorize]
        [HttpPatch]
        public async Task<IActionResult> Update([FromBody] Music st)
        {
            if (st == null)
            {
                return BadRequest("Music data is null.");
            }

            try
            {
                var existing = _context.Musics.Find(st.Id);

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

        [HttpGet("page/{page}")]
        public async Task<IActionResult> Page(int page)
        {
            if (page <= 0)
            {
                return BadRequest();
            }

            int skip = (page - 1) * pageSize;

            MusicResponse resp = new MusicResponse
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalResults = await _context.Musics.CountAsync(),
                Items = await _context.Musics.Skip(skip).Take(pageSize).ToListAsync()
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
                var query = _context.Musics.Where(st => st.Title.ToLower().Contains(lowerQ) || st.SubTitle.ToLower().Contains(lowerQ) || st.Author.ToLower().Contains(lowerQ));

                MusicResponse resp = new MusicResponse
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

