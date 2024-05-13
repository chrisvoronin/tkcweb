using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TKC.Data;

namespace TKC.Models
{
    public class HtmlContentViewComponent: ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public HtmlContentViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(object arguments)
        {
            dynamic args = arguments;
            int contentId = args.contentid;

            var content = await _context.HTMLContents.FirstOrDefaultAsync(f => f.Id == contentId);
            return View("_HtmlContent", content);
        }
    }
}

