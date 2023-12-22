using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TKC.Data;
using TKC.Models;

namespace TKC.Controllers
{

    [ApiController]
    [Route("api/staff")]
    public class ApiStaffController : Controller
    {
        private readonly CacheService _cache;
        private readonly ApplicationDbContext _context;
        private readonly YoutubeAPI _api;
        private readonly int pageSize = 10;

        public ApiStaffController(CacheService cache, YoutubeAPI api, ApplicationDbContext context)
        {
            _cache = cache;
            _context = context;
            _api = api;
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {

            try
            {
                var toDelete = _context.Employees.Find(id);

                if (toDelete != null)
                {
                    _context.Employees.Remove(toDelete);
                    await _context.SaveChangesAsync();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Read(int id)
        {
            try
            {
                var shortTake = await _context.Employees.FirstOrDefaultAsync(st => st.Id == id);

                if (shortTake == null)
                {
                    return NotFound();
                }

                return Json(shortTake);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }


        [HttpGet("page/{page}")]
        public async Task<IActionResult> Page(int page)
        {
            if (page <= 0)
            {
                return BadRequest();
            }

            int skip = (page - 1) * pageSize;

            EmployeeResponse resp = new EmployeeResponse
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalResults = await _context.Employees.CountAsync(),
                Items = await _context.Employees.Skip(skip).Take(pageSize).ToListAsync()
            };

            return Json(resp);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 3)
            {
                return BadRequest();
            }

            try
            {
                var lowerQ = q.ToLower();
                var query = _context.Employees.Where(st => st.Name.ToLower().Contains(lowerQ) || st.Description.ToLower().Contains(lowerQ));

                EmployeeResponse resp = new EmployeeResponse
                {
                    Items = await query.Take(pageSize).ToListAsync(),
                    CurrentPage = 1,
                    ItemsPerPage = pageSize,
                    TotalResults = await query.CountAsync()
                };

                return Json(resp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [Authorize]
        [HttpPost()]
        public async Task<IActionResult> Create([FromForm] IFormCollection formData)
        {
            var photo = formData.Files["photo"];

            if (photo == null)
            {
                return BadRequest("Photo file is required.");
            }

            string name = "";
            string description = "";
            string group = "";
            int order = 0;

            if (formData.ContainsKey("name"))
            {
                name = formData["name"].ToString();
            } else
            {
                return BadRequest("Name is required");
            }

            if (formData.ContainsKey("description"))
            {
                description = formData["description"].ToString();
            }
            else
            {
                return BadRequest("Description is required");
            }

            if (formData.ContainsKey("group"))
            {
                group = formData["group"].ToString();
            }
            else
            {
                return BadRequest("Group is required");
            }

            if (formData.ContainsKey("order"))
            {
                int.TryParse(formData["order"], out order);
            }
            else
            {
                return BadRequest("Order is required");
            }

            // Now handle filling out form
            string? photoFileName = null;

            try
            {
                if (photo != null)
                {
                    photoFileName = await FileUtility.SaveStaffPhoto(photo);
                }

                var existing = new Staff();

                existing.Name = name;
                existing.Group = group;
                existing.Description = description;
                existing.Order = order;

                if (photoFileName != null)
                    existing.PhotoUrl = photoFileName;

                _context.Employees.Add(existing);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(long id, [FromForm] IFormCollection formData)
        {
            var photo = formData.Files["photo"];

            string? name = null;
            string? description = null;
            string? group = null;
            int order = 0;

            if (formData.ContainsKey("name"))
            {
                name = formData["name"].ToString();
            }
            if (formData.ContainsKey("description"))
            {
                description = formData["description"].ToString();
            }
            if (formData.ContainsKey("group"))
            {
                group = formData["group"].ToString();
            }
            if (formData.ContainsKey("order"))
            {
                int.TryParse(formData["order"], out order);
            }
            
            // Now handle filling out form
            string? photoFileName = null;

            try
            {
                var existing = _context.Employees.Find(id);
                if (existing == null)
                {
                    return NotFound();
                }

                if (photo != null)
                {
                    photoFileName = await FileUtility.SaveStaffPhoto(photo);
                }

                if (name != null)
                    existing.Name = name;

                if (group != null)
                    existing.Group = group;

                if (description != null)
                    existing.Description = description;

                existing.Order = order;

                if (photoFileName != null)
                    existing.PhotoUrl = photoFileName;

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

    }
}

