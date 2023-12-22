using Microsoft.AspNetCore.Mvc;
using TKC.Data;

namespace TKC.Controllers
{
    [Route("shorttake")]
    public class ShortTakesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShortTakesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
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
    }
}

