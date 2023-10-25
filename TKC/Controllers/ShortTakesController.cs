using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TKC.Data;
using TKC.Models;

namespace TKC.Controllers
{
    [Route("[controller]")]
    public class ShortTakesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly int pageSize = 10;

        public ShortTakesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ShortTakeResponse resp = new ShortTakeResponse();
            resp.CurrentPage = 1;
            resp.ItemsPerPage = pageSize;
            resp.TotalResults = _context.ShortTakes.Count();
            resp.Items = _context.ShortTakes.Take(pageSize).ToList();
            return View(resp);
        }

        [HttpGet("{id}")]
        public IActionResult Detail(int id)
        {
            var m = _context.ShortTakes.FirstOrDefault(m => m.Id == id);
            if (m == null)
            {
                return NotFound();
            }
            return View(m);
        }

        // SECTION BELOW IS API
        // SECTION BELOW IS API
        // SECTION BELOW IS API

        [HttpGet("api/page/{page}")]
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

        [HttpGet("api/search")]
        public async Task<IActionResult> Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 3)
            {
                return BadRequest();
            }

            try
            {
                //var query = _context.ShortTakes.Where(st => st.Title.Contains(q) || st.Speaker.Contains(q));
                var lowerQ = q.ToLower();
                var query = _context.ShortTakes.Where(st => st.Title.ToLower().Contains(lowerQ) || st.Speaker.ToLower().Contains(lowerQ));


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

