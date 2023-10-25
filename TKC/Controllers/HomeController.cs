using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TKC.Data;
using TKC.Models;

namespace TKC.Controllers;

[Route("[action]")]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HomeController> _logger;

    private readonly CacheService _cache;
    private readonly YoutubeAPI _api;
    private readonly int sermonsPerPage = 10;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, CacheService cache, YoutubeAPI api)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
        _api = api;
    }

    [Route("~/")]
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Contact(string name, string email, string message)
    {
        List<AppSettingModel> settings = _context.AppSettings.ToList();
        string emailTo = settings.First(f => f.Key == "EmailTo").Value;
        string emailFrom = settings.First(f => f.Key == "EmailFrom").Value;
        string emailSubject = settings.First(f => f.Key == "EmailSubject").Value;

        var m = new Mailer();
        string text = $"Name: {name}, Email: {email}, Message: {message}";
        bool success = m.SendEmail(emailFrom, emailTo, emailSubject, text);
        if (success)
            return StatusCode(200);
        else
            return StatusCode(400);
    }

    public IActionResult Liturgy()
    {
        return View();
    }

    public async Task<IActionResult> Resources()
    {
        ResourcesViewModel vm = new ResourcesViewModel()
        {
            Music = _context.Musics.Take(3).ToList(),
            Sermons = new List<Sermon>(),
            Shorts = _context.ShortTakes.Take(3).ToList()
        };

        var apiResponse = await _api.GetPlaylistVideos(_cache, _api.PlayListIdSermons, sermonsPerPage);
        if (apiResponse != null)
        {
            var conv = SermonConverter.Convert(apiResponse);
            vm.Sermons = conv.Sermons.Take(6).ToList();
        }

        return View(vm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

