using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TKC.Data;
using TKC.Models;
using TKC.Models.PlanningCenter;
using System.Linq;

namespace TKC.Controllers;

[Route("[action]")]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HomeController> _logger;

    private readonly CacheService _cache;
    private readonly YoutubeAPI _api;
    private readonly PlanningCenterService _planningService;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, CacheService cache, YoutubeAPI api, PlanningCenterService planningService)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
        _api = api;
        _planningService = planningService;
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

    public async Task<IActionResult> Groups()
    {
        //todo: filter only open, not closed
        var groups = await _planningService.GetGroupsResponseAsync();
        //filter groups
        groups = groups ?? new Models.PlanningCenter.GroupsResponse();
        //has to have schedule
        groups.data = groups.data.Where(g => g.attributes.schedule != null && g.attributes.description != null).ToList();

        var enrollmentAll = groups.included ?? new List<EnrollmentData>();
        var groupsAll = groups.data;

        // Combine groups and enrollments
        var groupWithEnrollments = new List<GroupWithEnrollment>();
        foreach (var group in groupsAll)
        {
            foreach (var enrollment in enrollmentAll)
            {
                if (group.id == enrollment.id)
                {
                    groupWithEnrollments.Add(new GroupWithEnrollment
                    {
                        group = group,
                        enrollment = enrollment.attributes
                    });
                }
            }
        }

        var activeGroups = groupWithEnrollments.Where(g => g.enrollment.status != "closed").ToList();
        
        var parishData = activeGroups.Where(g => g.group.attributes.name.Contains("Parish")).ToList();
        var otherData = activeGroups.Where(g => !g.group.attributes.name.Contains("Parish")).ToList();

        //now lets put them into buckets
        Tuple<string, List<GroupWithEnrollment>> parish = new Tuple<string, List<GroupWithEnrollment>>("Parish Groups", parishData);
        Tuple<string, List<GroupWithEnrollment>> other = new Tuple<string, List<GroupWithEnrollment>>("Other Groups", otherData);
        List<Tuple<string, List<GroupWithEnrollment>>> list = new List<Tuple<string, List<GroupWithEnrollment>>>();
        list.Add(parish);
        if (otherData.Count > 0)
            list.Add(other);
        return View(list);
    }

    public IActionResult Resources()
    {

        ResourcePageModel model = new ResourcePageModel();
        var allGroups = _context.ResourceGroups
                .Include(group => group.Items)
                .ToList();

        var governingDocs = allGroups.Where(f => f.Side == 0).ToList();
        var policies = allGroups.Where(f => f.Side == 1).ToList();

        //return View(model);

        ResourcesPageModel m = new ResourcesPageModel();
        foreach(var item in governingDocs)
        {
            foreach (var i in item.Items)
            {
                KeyValuePair<string, string> b = new KeyValuePair<string, string>(i.Text, buildUrl(i));
                m.governingDocs.Add(b);
            }
        }
        foreach(var item in policies)
        {
            foreach (var i in item.Items)
            {
                KeyValuePair<string, string> b = new KeyValuePair<string, string>(i.Text, buildUrl(i));
                m.policies.Add(b);
            }
        }
        //model.policies.Add(new KeyValuePair<string, string>("Music Policy", "/TKC-Policy-Music-Ministry.pdf"));
        //model.policies.Add(new KeyValuePair<string, string>("Adult Children Membership Policy", "/Children-of-Members-Protocol.pdf"));

        //model.governingDocs.Add(new KeyValuePair<string, string>("TKC Constitution", "/KINGS-CONSTITUTION.pdf"));
        //model.governingDocs.Add(new KeyValuePair<string, string>("TKC Covenant", "/KINGS-COVENANT.pdf"));
        //model.governingDocs.Add(new KeyValuePair<string, string>("TKC Confession", "/KINGS-CONFESSION-OF-FAITH-2022.pdf"));
        //model.governingDocs.Add(new KeyValuePair<string, string>("TKC CREC Governing Documents", "https://crechurches.org/documents/governance/CREC_Governance_Comprehensive_2017R.pdf"));

        return View(m);
    }

    private string buildUrl(ResourceItem item)
    {
        if (item.FileName.StartsWith("http"))
        {
            return item.FileName;
        }
        else
        {
            return "/File/Documents/" + item.FileName;
        }
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

