﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using TKC.Data;
using TKC.Models;

namespace TKC.Controllers
{

    [ApiController]
    [Route("api/settings")]
    public class ApiSettingsController : Controller
    {
        private readonly CacheService _cache;
        private readonly ApplicationDbContext _context;

        public ApiSettingsController(CacheService cache, ApplicationDbContext context)
        {
            _cache = cache;
            _context = context;
        }

        [Authorize]
        [HttpPatch("logins/{id}/{confirmed}")]
        public async Task<IActionResult> UpdateLogin(string id, bool confirmed)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Invalid Username");
            }

            var currUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currUserId == id)
            {
                return BadRequest("Can't update your own account.");
            }

            try
            {
                var existing = _context.Users.Find(id);

                if (existing == null)
                {
                    return NotFound();
                }

                existing.EmailConfirmed = confirmed;
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }

        }

        [Authorize]
        [HttpPatch("{key}")]
        public async Task<IActionResult> Update(string key, [FromForm] IFormCollection formData)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return BadRequest("Key can't be blank or empty space");
            }

            if (!formData.ContainsKey("value"))
            {
                return BadRequest("Value can't be blank or empty space");
            }

            string value = formData["value"]!;
            
            if (string.IsNullOrWhiteSpace(value))
            {
                return BadRequest("Value can't be blank or empty space");
            }

            try
            {
                var existing = _context.AppSettings.Find(key);

                if (existing == null)
                {
                    return NotFound();
                }

                existing.Value = value;
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

