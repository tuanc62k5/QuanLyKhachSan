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

    // ===== ĐĂNG KÝ =====
    public IActionResult DangKy()
    {
        return View();
    }

    [HttpPost]
    public IActionResult DangKy(tblKhachHang kh)
    {
        // check email tồn tại
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

    // ===== ĐĂNG NHẬP =====
    public IActionResult DangNhap()
    {
        return View();
    }

    [HttpPost]
    public IActionResult DangNhap(string email, string password)
    {
        var user = _context.KhachHangs
            .FirstOrDefault(x => x.KH_Email == email && x.KH_MatKhau == password);

        if (user == null)
        {
            ViewBag.Error = "Sai email hoặc mật khẩu!";
            return View();
        }

        // lưu session
        HttpContext.Session.SetString("UserName", user.KH_TenKhach);
        HttpContext.Session.SetInt32("UserID", user.KH_ID);

        return RedirectToAction("Index", "Home");
    }

    // ===== ĐĂNG XUẤT =====
    public IActionResult DangXuat()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}