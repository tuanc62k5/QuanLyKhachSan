using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DoAn.Models;

namespace aznews.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;

    public HomeController(ILogger<HomeController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

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

    [Route("/room-{id:long}.html")]
    public IActionResult Details(long id)
    {
        var room = _context.Rooms.FirstOrDefault(x => x.RoomID == id);

        var reviews = _context.Reviews
            .Where(r => r.RoomID == id)
            .OrderByDescending(r => r.CreatedDate)
            .ToList();

        ViewBag.Reviews = reviews;

        return View(room);
    }
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}