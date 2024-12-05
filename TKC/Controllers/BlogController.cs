using System.Drawing.Printing;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TKC.Data;
using TKC.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TKC.Controllers
{
    [Route("[controller]")]
    public class BlogController : Controller
    {
        private readonly int pageSize = 10;
        private readonly ApplicationDbContext _context;

        public BlogController(ApplicationDbContext context)
        {
            //should page.
            _context = context;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var result = await _context.BlogCategories.ToListAsync();
            return View(result);
        }

        [HttpGet("page/{page}")]
        public async Task<IActionResult> Page(int page)
        {
            if (page <= 0)
            {
                return BadRequest();
            }

            int skip = (page - 1) * pageSize;

            PagedResponse<BlogPost> resp = new PagedResponse<BlogPost>
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalResults = await _context.Blogs.Where(b => b.status == 1).CountAsync(),
                Items = await _context.Blogs.Where(b => b.status == 1).OrderByDescending(i => i.id).Skip(skip).Take(pageSize).ToListAsync()
            };
            foreach(var item in resp.Items)
            {
                item.html = GetPreviewText(item.html ?? "", 200);
            }

            return Json(resp);
        }

        [HttpGet("{id}/{title?}")]
        public IActionResult Detail(int id, string? title = null)
        {
            var m = _context.Blogs.FirstOrDefault(m => m.id == id && m.status == 1);

            // status of 0 == not public
            if (m == null)
            {
                return NotFound();
            }

            string desiredUriTitle = (m.title ?? "").ToLower().Replace(" ", "-");
            if (title != desiredUriTitle)
            {
                return RedirectToAction("Detail", new { id, title = desiredUriTitle });
            }

            return View(m);
        }

        [HttpGet("topic/{topic}/{title?}")]
        public async Task<IActionResult> Topic(int topic, string? title = null)
        {
            var item = await _context.BlogCategories.Where(f => f.id == topic).FirstOrDefaultAsync();

            if (item == null)
                return NotFound();

            var allTopics = await _context.BlogCategories.ToListAsync();
            TopicsPageModel result = new TopicsPageModel()
            {
                allTopics = allTopics,
                result = item
            };
            return View(result);
        }

        

        [HttpGet("pagetopic/{topic}/{page}")]
        public async Task<IActionResult> PageTopic(int topic, int page)
        {
            if (page <= 0)
            {
                return BadRequest();
            }

            int skip = (page - 1) * pageSize;

            PagedResponse<BlogPost> resp = new PagedResponse<BlogPost>
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalResults = await _context.Blogs.CountAsync(),
                Items = await _context.Blogs.Where(b => b.categoryId == topic && b.status == 1).OrderByDescending(i => i.id).Skip(skip).Take(pageSize).ToListAsync()
            };
            foreach (var item in resp.Items)
            {
                item.html = GetPreviewText(item.html ?? "", 200);
            }

            return Json(resp);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> Categories()
        {
            var cats = await _context.BlogCategories.Where(f => f.flags == 1).ToListAsync();
            return new JsonResult(cats);
        }

        [HttpGet("recentposts")]
        public async Task<IActionResult> RecentPosts()
        {
            var recentPosts = await _context.Blogs
                        .OrderByDescending(blog => blog.dateCreated) // Use CreatedDate or another timestamp property
                        .Where(b => b.status == 1)
                        .Take(5) // Limit to the last 5 posts
                        .Select(blog => new { blog.id, blog.title }) // Project only Id and Title
                        .ToListAsync(); // Execute the query
            return new JsonResult(recentPosts);
        }

        private string GetPreviewText(string htmlContent, int characterLimit = 200)
        {
            if (string.IsNullOrEmpty(htmlContent))
                return string.Empty;

            // Load the HTML into HtmlDocument
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            // Extract the plain text
            var plainText = htmlDoc.DocumentNode.InnerText;

            // Truncate to the specified character limit
            if (plainText.Length > characterLimit)
                plainText = plainText.Substring(0, characterLimit) + "...";

            return plainText;
        }

    }
}

