﻿using Microsoft.AspNetCore.Authorization;
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

        public AdminController(CacheService cache, YoutubeAPI api, ApplicationDbContext context)
        {
            _cache = cache;
            _context = context;
            _api = api;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //int sermonCount = 0;
            //var apiResponse = await _api.GetPlaylistVideos(_cache, _api.PlayListIdSermons, 1);
            //if (apiResponse != null)
            //{
            //    var resp = SermonConverter.Convert(apiResponse);
            //    sermonCount = resp.TotalResults;
            //}

            AdminSummary sum = new AdminSummary()
            {
                MusicCount = await _context.Musics.CountAsync(),
                ShortTakeCount = await _context.ShortTakes.CountAsync(),
                SermonsCount = await _context.Sermons.CountAsync(),
                StaffCount = await _context.Employees.CountAsync()
            };

            return View(sum);
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

    }
}

