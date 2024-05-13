using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TKC.Data;
using TKC.Models;

namespace TKC.Controllers
{

    [Route("podcasts")]
    public class PodcastsController : Controller
    {
        private readonly CacheService _cache;
        private readonly ApplicationDbContext _context;

        public PodcastsController(CacheService cache, ApplicationDbContext context)
        {
            _cache = cache;
            _context = context;
        }

        [HttpGet("sermons")]
        public IActionResult Sermons()
        {
            string fileName = "podcast.xml";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName);
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            return File(fileStream, "application/xml", "podcast.xml");
        }

        [HttpHead("sermons")]
        public IActionResult SermonsHeadInfo()
        {
            string fileName = "podcast.xml";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            long fileSize = new FileInfo(filePath).Length;

            Response.Headers.Add("Content-Type", "application/xml");
            Response.Headers.Add("Content-Length", $"{fileSize}");
            return Ok();
        }

        [Authorize]
        [HttpGet("generate")]
        public async Task<IActionResult> Generate()
        {
            var sermons = await _context.Sermons.ToListAsync();
            var shorts = await _context.ShortTakes.ToListAsync();

            PodcastXMLGenerator generator = new PodcastXMLGenerator();
            var doc = generator.GenerateXml(sermons, shorts);
            await FileUtility.SavePodcastXml(doc);
            return NoContent();
        }
    }
}

