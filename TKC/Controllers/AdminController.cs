using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TKC.Data;
using TKC.Models;

namespace TKC.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class AdminController : Controller
    {
        private readonly CacheService _cache;
        private readonly ApplicationDbContext _context;
        private readonly YoutubeAPI _api;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(CacheService cache, YoutubeAPI api
            , ApplicationDbContext context
            , UserManager<IdentityUser> userManager
            , RoleManager<IdentityRole> roleManager)
        {
            _cache = cache;
            _context = context;
            _api = api;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            AdminSummary sum = new AdminSummary()
            {
                MusicCount = await _context.Musics.CountAsync(),
                ShortTakeCount = await _context.ShortTakes.CountAsync(),
                SermonsCount = await _context.Sermons.CountAsync(),
                StaffCount = await _context.Employees.CountAsync(),
                Settings = await _context.AppSettings.CountAsync(),
                Logins = await _context.Users.CountAsync(),
                HtmlContent = await _context.HTMLContents.CountAsync(),
                Resources = await _context.Resources.CountAsync()
            };

            return View(sum);
        }

        private async Task Stuffstuff()
        {
            IdentityRole role = new IdentityRole("Admin");
            await _roleManager.CreateAsync(role);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userId);
            await _userManager.AddToRoleAsync(user, "Admin");
        }

        // SHORT TAKES SHORT TAKES SHORT TAKES SHORT TAKES SHORT TAKES SHORT TAKES SHORT TAKES SHORT TAKES SHORT TAKES SHORT TAKES SHORT TAKES 

        // List
        [HttpGet("ShortTake")]
        public IActionResult ShortTakeList()
        {
            return View();
        }

        // View Single
        [HttpGet("ShortTake/{id}")]
        public async Task<IActionResult> ShortTake(int id)
        {
            var m = await _context.ShortTakes.FirstOrDefaultAsync(m => m.Id == id);
            if (m == null)
            {
                return NotFound();
            }
            return View(m);
        }

        [HttpGet("ShortTake/new")]
        public IActionResult ShortTakeNew()
        {
            return View();
        }

        // MUSIC MUSIC MUSIC MUSIC MUSIC MUSIC MUSIC MUSIC MUSIC MUSIC MUSIC MUSIC MUSIC MUSIC MUSIC MUSIC MUSIC MUSIC MUSIC MUSIC MUSIC MUSIC

        // List
        [HttpGet("Music")]
        public IActionResult MusicList()
        {
            return View();
        }

        // View Single
        [HttpGet("Music/{id}")]
        public async Task<IActionResult> Music(int id)
        {
            var m = await _context.Musics.FirstOrDefaultAsync(m => m.Id == id);
            if (m == null)
            {
                return NotFound();
            }
            return View(m);
        }

        // Add New
        [HttpGet("Music/new")]
        public IActionResult MusicNew()
        {
            return View();
        }

        // SERMONS SERMONS SERMONS SERMONS SERMONS SERMONS SERMONS SERMONS SERMONS SERMONS SERMONS SERMONS SERMONS SERMONS SERMONS SERMONS SERMONS SERMONS SERMONS

        // List
        [HttpGet("Sermon")]
        public IActionResult SermonList()
        {
            return View();
        }

        // View Single
        [HttpGet("Sermon/{id}")]
        public async Task<IActionResult> Sermon(int id)
        {
            var m = await _context.Sermons.FirstOrDefaultAsync(m => m.Id == id);
            if (m == null)
            {
                return NotFound();
            }
            return View(m);
        }

        // Add New
        [HttpGet("Sermon/new")]
        public IActionResult SermonNew()
        {
            return View();
        }

        // STAFF STAFF STAFF STAFF STAFF STAFF STAFF STAFF STAFF STAFF STAFF STAFF STAFF STAFF STAFF STAFF STAFF

        // List
        [HttpGet("Staff")]
        public IActionResult StaffList()
        {
            return View();
        }

        // View Single
        [HttpGet("Staff/{id}")]
        public async Task<IActionResult> Staff(int id)
        {
            var m = await _context.Employees.FirstOrDefaultAsync(m => m.Id == id);
            if (m == null)
            {
                return NotFound();
            }
            return View(m);
        }

        // Add New
        [HttpGet("Staff/new")]
        public IActionResult StaffNew()
        {
            return View();
        }

        [HttpGet("Settings")]
        public IActionResult Settings()
        {
            List<AppSettingModel> items = _context.AppSettings.OrderBy(i => i.Key).ToList();
            return View(items);
        }

        [HttpGet("Logins")]
        public async Task<IActionResult> Logins()
        {
            var users = _context.Users.OrderBy(i => i.UserName).ToList();
            List<UserDisplay> res = new();
            foreach(var u in users)
            {
                string email = u.Email ?? "";
                var identityUser = await _userManager.FindByEmailAsync(email);
                bool userHasRole = await _userManager.IsInRoleAsync(identityUser, "Admin");

                var user = new UserDisplay()
                {
                    Id = u.Id,
                    Username = u.Email ?? "NoEmail",
                    IsActive = u.EmailConfirmed,
                    IsAdmin = userHasRole,
                    LockOutDate = u.LockoutEnd?.DateTime
                };
                res.Add(user);
            }
            
            return View(res);
        }

        /// HTML CONTENT
        ///
        [HttpGet("htmlcontent")]
        public IActionResult HtmlContent()
        {
            List<HTMLContent> items = _context.HTMLContents.OrderBy(i => i.Name).ToList();
            return View(items);
        }

        [HttpGet("htmlcontent/new")]
        public IActionResult HtmlContentNew()
        {
            return View();
        }

        [HttpGet("htmlcontent/{id}")]
        public async Task<IActionResult> HtmlContent(int id)
        {
            var m = await _context.HTMLContents.FirstOrDefaultAsync(m => m.Id == id);
            if (m == null)
            {
                return NotFound();
            }
            return View("HtmlContentDetail", m);
        }

        [HttpGet("Resources")]
        public IActionResult ResourceList()
        {
            ResourcePageModel model = new ResourcePageModel();
            var allGroups = _context.ResourceGroups
                    .Include(group => group.Items) // Eager loading Items navigation property
                    .ToList();

            model.LeftSide = allGroups.Where(f => f.Side == 0).ToList();
            model.RightSide = allGroups.Where(f => f.Side == 1).ToList();

            return View(model);
        }

        // View Single
        [HttpGet("Resources/{id}")]
        public async Task<IActionResult> Resource(int id)
        {
            var m = await _context.Resources.FirstOrDefaultAsync(m => m.Id == id);
            if (m == null)
            {
                return NotFound();
            }
            var allGroups = _context.ResourceGroups.ToList();

            ResourceDetailPageModel pageModel = new ResourceDetailPageModel();
            pageModel.Item = m;
            pageModel.Groups = allGroups;

            return View(pageModel);
        }

        [HttpGet("Resources/new")]
        public IActionResult ResourceNew()
        {
            var allGroups = _context.ResourceGroups.ToList();
            return View(allGroups);
        }

    }
}

