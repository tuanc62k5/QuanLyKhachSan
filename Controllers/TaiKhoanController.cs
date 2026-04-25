using Microsoft.AspNetCore.Mvc;
using DoAn.Models;
using System.Linq;

public class TaiKhoanController : Controller
{
    private readonly AppDbContext _context;

    public TaiKhoanController(AppDbContext context)
    {
        _context = context;
    }
    public IActionResult DangKy()
    {
        return View();
    }

    [HttpPost]
    public IActionResult DangKy(tblKhachHang kh)
    {
        if (_context.KhachHangs.Any(x => x.KH_Email == kh.KH_Email))
        {
            ViewBag.Error = "Email đã tồn tại!";
            return View();
        }

        _context.KhachHangs.Add(kh);
        _context.SaveChanges();

        TempData["Success"] = "Đăng ký thành công!";
        return RedirectToAction("DangNhap");
    }
    public IActionResult DangNhap()
    {
        return View();
    }

    [HttpPost]
    public IActionResult DangNhap(string email, string password)
    {
        var user = _context.KhachHangs.FirstOrDefault(x => x.KH_Email == email && x.KH_MatKhau == password);

        if (user == null)
        {
            ViewBag.Error = "Sai email hoặc mật khẩu!";
            return View();
        }
        HttpContext.Session.SetString("UserName", user.KH_TenKhach);
        HttpContext.Session.SetInt32("UserID", user.KH_ID);

        return RedirectToAction("Index", "Home");
    }
    public IActionResult DangXuat()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
    public IActionResult TrangCaNhan()
    {
        var userId = HttpContext.Session.GetInt32("UserID");

        if (userId == null)
            return RedirectToAction("DangNhap");

        var user = _context.KhachHangs.FirstOrDefault(x => x.KH_ID == userId);

        return View(user);
    }
    public IActionResult LichSu()
    {
        var userId = HttpContext.Session.GetInt32("UserID");

        if (userId == null)
            return RedirectToAction("DangNhap");

        var list = _context.DatPhongs.Where(x => x.KH_ID == userId).ToList();

        return View(list);
    }
}