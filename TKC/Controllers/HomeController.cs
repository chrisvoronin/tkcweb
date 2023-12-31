﻿using System.Diagnostics;
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

    public async Task<IActionResult> Staff()
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

            Secretaries = await _context.Employees
                .Where(e => e.Group == "Secretary")
                .OrderBy(e => e.Order)
                .ToListAsync()
        };

        return View(vm);
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

    public IActionResult Liturgy()
    {
        return View();
    }

    public async Task<IActionResult> Resources()
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

