using Microsoft.AspNetCore.Mvc;
using TKC.Data;

namespace TKC.Controllers
{
    [Route("[controller]")]
    public class MusicController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MusicController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Pass the data and paging information to the view
            return View();
        }

        [HttpGet("{id}")]
        public IActionResult Detail(int id)
        {
            var m = _context.Musics.FirstOrDefault(m => m.Id == id);
            if (m == null)
            {
                return NotFound();
            }
            return View(m);
        }
    }
}

