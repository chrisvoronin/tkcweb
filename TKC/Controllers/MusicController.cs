using Microsoft.AspNetCore.Mvc;
using TKC.Data;
using TKC.Models;

namespace TKC.Controllers
{
    [Route("[controller]")]
    public class MusicController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly int pageSize = 2;

        public MusicController(ApplicationDbContext context)
        {
            _context = context;
        }

        

        [HttpGet]
        public IActionResult Index(int page = 1)
        {
            if (page < 1)
            {
                page = 1;
            }

            
            // Calculate the number of items to skip
            int skip = (page - 1) * pageSize;
            int totalCount = _context.Musics.Count();
            var paginatedData = _context.Musics.Skip(skip).Take(pageSize).ToList();
            var pageCount = (int)Math.Ceiling((double)totalCount / pageSize);
 
            var vm = new MusicViewModel() { TotalCount = totalCount, Data = paginatedData, PageCount = pageCount, CurrentPage = page };
            vm.PageSize = pageSize;
            vm.PreviousPage = page - 1;
            vm.NextPage = page + 1;
            if (vm.NextPage > vm.PageCount)
            {
                vm.NextPage = 0;
            }

            // Pass the data and paging information to the view
            return View(vm);
        }

        [HttpGet("Page")]
        public IActionResult Page(int page)
        {
            if (page <= 0)
            {
                return BadRequest();
            }

            // Calculate the number of items to skip
            int skip = (page - 1) * pageSize;

            // Retrieve paginated data from the database
            var data = _context.Musics.Skip(skip).Take(pageSize).ToList();

            // Pass the data and paging information to the view
            return Json(data);
        }

        [HttpGet("Search")]
        public IActionResult Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 3)
            {
                return NotFound();
            }

            var data = _context.Musics.Where(m => m.Title.Contains(q)).Take(pageSize).ToList();

            return Json(data);
        }

        [HttpGet("{id}")]
        public IActionResult Detail(int id)
        {
            var m = GetMusicById(id);
            if (m == null)
            {
                return NotFound();
            }
            //Video or audio view
            return View(m);
        }

        // Route: /music/{id}/mp3
        [HttpGet("{id}/mp3")]
        public IActionResult MP3(int id)
        {
            // Generate or fetch the PDF file based on the 'id'
            byte[] pdfBytes = GeneratePdf(id);

            // Specify the content type for PDF
            const string contentType = "application/pdf";

            // Return the PDF file as a download
            return File(pdfBytes, contentType, $"music_{id}.pdf");
        }

        // Route: /music/{id}/pdf
        [HttpGet("{id}/pdf")]
        public IActionResult PDF(int id)
        {
            var m = GetMusicById(id);
            // Generate or fetch the PDF file based on the 'id'
            byte[] pdfBytes = GeneratePdf(id);

            // Specify the content type for PDF
            const string contentType = "application/pdf";

            // Return the PDF file as a download
            return File(pdfBytes, contentType, $"music_{id}.pdf");
        }


        // Dummy method to generate PDF bytes (replace this with your logic)
        private byte[] GeneratePdf(int id)
        {
            // Replace this with your logic to generate or fetch the PDF
            // For simplicity, using a dummy content here
            string dummyContent = $"PDF content for music {id}";
            return System.Text.Encoding.UTF8.GetBytes(dummyContent);
        }

        private Music? GetMusicById(int id)
        {
            return _context.Musics.FirstOrDefault(m => m.Id == id);
        }
    }
}

