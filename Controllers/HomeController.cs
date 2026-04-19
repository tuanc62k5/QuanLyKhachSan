using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DoAn.Models;
using System.Linq;

namespace DoAn.Controllers; // ✔ sửa lại namespace

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

    // 👉 Chi tiết phòng
    [Route("/Phong-{id:long}.html")]
    public IActionResult Details(long id)
    {
        var phong = _context.Phongs
            .FirstOrDefault(x => x.P_ID == id);

        if (phong == null)
        {
            return NotFound();
        }

        // 👉 Nếu bảng review bạn chưa sửa thì giữ RoomID
        var gioiThieus = _context.GioiThieus
            .Where(gt => gt.P_ID == id)
            .OrderByDescending(gt => gt.GT_NgayTao)
            .ToList();

        ViewBag.GioiThieus = gioiThieus;

        return View(phong);
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel 
        { 
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
        });
    }
}