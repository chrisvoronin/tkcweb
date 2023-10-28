using Microsoft.AspNetCore.Mvc;
using TKC.Data;
using TKC.Models;

namespace TKC.Controllers
{

    [Route("sermon")]
    public class SermonsController : Controller
    {
        private readonly CacheService _cache;
        private readonly ApplicationDbContext _context;
        private readonly YoutubeAPI _api;
        private readonly int sermonsPerPage = 10;

        public SermonsController(CacheService cache, YoutubeAPI api, ApplicationDbContext context)
        {
            _cache = cache;
            _context = context;
            _api = api;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
            //var apiResponse = await _api.GetPlaylistVideos(_cache, _api.PlayListIdSermons, sermonsPerPage, p);
            //if (apiResponse == null)
            //{
            //    return View(new SermonListResponse());
            //}
            //else
            //{
            //    var resp = SermonConverter.Convert(apiResponse);
            //    if (pn != null)
            //    {
            //        resp.CurrentPage = (int)pn;
            //    }

            //    return View(resp);
            //}
        }

        [HttpGet("{id}")]
        public IActionResult Detail(int id)
        {
            //var item = _cache.Get<YouTubeVideo>(id);
            //if (item == null)
            //{
            //    //TODO: Get video details
            //    return Redirect("/Sermons");
            //}

            //var serm = SermonConverter.Convert(item);

            var m = _context.Sermons.FirstOrDefault(m => m.Id == id);
            if (m == null)
            {
                return NotFound();
            }
            return View(m);
        }

        [HttpGet("playlist")]
        public async Task<JsonResult> PlayList(string? pageToken = null)
        {
            var response = await _api.GetPlaylistVideos(_cache, _api.PlayListIdSermons, sermonsPerPage, pageToken);
            return Json(response);
        }

        
    }
}

