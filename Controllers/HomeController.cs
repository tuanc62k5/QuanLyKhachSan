using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DoAn.Models;
using DoAn.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Controllers;

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
        ViewBag.DichVus = _context.DichVus.Take(6).ToList();
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

    [Route("/Phong-{id:long}.html")]
    public IActionResult Details(long id)
    {
        var phong = _context.Phongs.FirstOrDefault(x => x.P_ID == id);

        if (phong == null)
        {
            return NotFound();
        }
        var gioiThieus = _context.GioiThieus.Where(gt => gt.P_ID == id)
        .OrderByDescending(gt => gt.GT_NgayTao).ToList();

        ViewBag.GioiThieus = gioiThieus;

        ViewBag.DanhGia = _context.DanhGias.Include(x => x.KhachHang).Where(x => x.P_ID == id)
        .OrderByDescending(x => x.DG_NgayTao).ToList();

        return View(phong);
    }

    [HttpPost]
    public IActionResult DanhGia(int P_ID, int DG_Sao, string DG_NoiDung)
    {
        var khId = HttpContext.Session.GetInt32("UserID");

        if (khId == null)
        {
            TempData["Error"] = "Vui lòng đăng nhập!";
            return RedirectToAction("Details", new { id = P_ID });
        }

        var dg = new tblDanhGia
        {
            P_ID = P_ID,
            KH_ID = khId.Value,
            DG_Sao = DG_Sao,
            DG_NoiDung = DG_NoiDung,
            DG_NgayTao = DateTime.Now
        };

        _context.DanhGias.Add(dg);
        _context.SaveChanges();

        TempData["Success"] = "Đánh giá thành công!";

        return Redirect($"/Phong-{P_ID}.html");
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}