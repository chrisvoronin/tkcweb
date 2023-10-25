using Microsoft.AspNetCore.Mvc;
using TKC.Data;
using TKC.Models;

namespace TKC.Controllers
{

    [Route("[controller]")]
    //[Authorize]
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
        public async Task<IActionResult> Index(string? p = null, int? pn = null)
        {
            var apiResponse = await _api.GetPlaylistVideos(_cache, _api.PlayListIdSermons, sermonsPerPage, p);
            if (apiResponse == null)
            {
                return View(new SermonListResponse());
            }
            else
            {
                var resp = SermonConverter.Convert(apiResponse);
                if (pn != null)
                {
                    resp.CurrentPage = (int)pn;
                }

                return View(resp);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Detail(string id)
        {
            var item = _cache.Get<YouTubeVideo>(id);
            if (item == null)
            {
                //TODO: Get video details
                return Redirect("/Sermons");
            }

            var serm = SermonConverter.Convert(item);
            return View(serm);
        }

        [HttpGet("playlist")]
        public async Task<JsonResult> PlayList(string? pageToken = null)
        {
            var response = await _api.GetPlaylistVideos(_cache, _api.PlayListIdSermons, sermonsPerPage, pageToken);
            return Json(response);
        }

        
    }
}

