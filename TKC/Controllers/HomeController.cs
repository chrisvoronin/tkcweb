using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    public IActionResult Resources()
    {
        ResourcesPageModel model = new ResourcesPageModel();
        model.policies.Add(new KeyValuePair<string, string>("Music Policy", "/TKC-Policy-Music-Ministry.pdf"));
        model.policies.Add(new KeyValuePair<string, string>("Adult Children Membership Policy", "/Children-of-Members-Protocol.pdf"));

        model.governingDocs.Add(new KeyValuePair<string, string>("TKC Constitution", "/KINGS-CONSTITUTION.pdf"));
        model.governingDocs.Add(new KeyValuePair<string, string>("TKC Covenant", "/KINGS-COVENANT.pdf"));
        model.governingDocs.Add(new KeyValuePair<string, string>("TKC Confession", "/KINGS-CONFESSION-OF-FAITH-2022.pdf"));
        model.governingDocs.Add(new KeyValuePair<string, string>("TKC CREC Governing Documents", "https://crechurches.org/documents/governance/CREC_Governance_Comprehensive_2017R.pdf"));



        return View(model);
    }

    public IActionResult LordsDayWorship()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Contact(string name, string email, string message)
    {
        List<AppSettingModel> settings = _context.AppSettings.ToList();
        string to = settings.First(f => f.Key == "EmailTo").Value;
        string subject = settings.First(f => f.Key == "EmailSubject").Value;
        string body = $"Name: {name}, Email: {email}, Message: {message}";
        string from = settings.First(f => f.Key == "EmailFrom").Value;
        string password = settings.First(f => f.Key == "EmailPassword").Value;
        string host = settings.First(f => f.Key == "EmailHost").Value;
        int port = Convert.ToInt32(settings.First(f => f.Key == "EmailPort").Value); ;

        var m = new Mailer();
        bool success = m.SendEmail(to, subject, body, from, password, host, port);
        if (success)
            return StatusCode(200);
        else
            return StatusCode(400);
    }

    public async Task<IActionResult> Liturgy()
    {
        var m = _cache.Get<HTMLContent>("Liturgy");
        if (m == null)
        {
            m = await _context.HTMLContents.FirstAsync(m => m.Name == "Liturgy");
            _cache.Set("Liturgy", m);
        }
        return View(m);
    }

    public async Task<IActionResult> WhoWeAre()
    {
        var vm = new StaffViewModel()
        {
            Deacons = await _context.Employees
                .Where(e => e.Group == "Deacon")
                .OrderBy(e => e.Order)
                .ToListAsync(),

            Elders = await _context.Employees
                .Where(e => e.Group == "Elder")
                .OrderBy(e => e.Order)
                .ToListAsync(),

            Pastoral = await _context.Employees
                .Where(e => e.Group == "Pastoral")
                .OrderBy(e => e.Order)
                .ToListAsync(),

            Secretaries = await _context.Employees
                .Where(e => e.Group == "Secretary")
                .OrderBy(e => e.Order)
                .ToListAsync()
        };

        return View(vm);
    }

    public async Task<IActionResult> Media()
    {
        ResourcesViewModel vm = new ResourcesViewModel()
        {
            Music = await _context.Musics.OrderByDescending(i => i.Id).Take(3).ToListAsync(),
            Sermons = await _context.Sermons.OrderByDescending(i => i.Id).Take(6).ToListAsync(),
            Shorts = await _context.ShortTakes.OrderByDescending(i => i.Id).Take(3).ToListAsync()
        };

        //var apiResponse = await _api.GetPlaylistVideos(_cache, _api.PlayListIdSermons, sermonsPerPage);
        //if (apiResponse != null)
        //{
        //    var conv = SermonConverter.Convert(apiResponse);
        //    vm.Sermons = conv.Sermons.Take(6).ToList();
        //}

        return View(vm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

