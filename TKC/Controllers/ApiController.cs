using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TKC.Data;
using TKC.Models;
using System.Text.RegularExpressions;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TKC.Controllers
{
    [ApiController]
    
    [Route("[controller]")]
    public class ApiController : Controller
    {
        private readonly CacheService _cache;
        private readonly ApplicationDbContext _context;
        private readonly YoutubeAPI _api;

        public ApiController(CacheService cache, YoutubeAPI api, ApplicationDbContext context)
        {
            _cache = cache;
            _context = context;
            _api = api;
        }

        [HttpGet("join")]
        public async Task<IActionResult> JoinMailingList([FromQuery] string email)
        {

            if (!IsValidEmail(email))
                return Ok();

            try
            {
                EmailSignUp signUp = new EmailSignUp() { email = email, dateCreated = DateTime.Now };
                _context.EmailSignUps.Add(signUp);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
            return Ok();
        }

        public static bool IsValidEmail(string email)
        {
            var regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            return regex.IsMatch(email);
        }

        [Authorize]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                // Check if the file is null
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Please select a file.");
                }

                // Define a unique file name
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                // Define the path to save the file
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName);

                // Create the directory if it doesn't exist
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                // Save the file to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // You can save additional information about the file to a database or perform other actions here

                return Ok(new { FileName = fileName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        

    }
}

