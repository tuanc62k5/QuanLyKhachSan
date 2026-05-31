using Microsoft.AspNetCore.Mvc;
using DoAn.Models;
using DoAn.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;

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

        var list = _context.DatPhongs
            .Where(x => x.KH_ID == userId)
            .Include(x => x.Phong)
            .ToList();

        return View(list);
    }

    public IActionResult DichVuDaDat()
    {
        var userId = HttpContext.Session.GetInt32("UserID");

        if (userId == null)
            return RedirectToAction("DangNhap");

        var list = _context.SuDungDichVus.Include(x => x.DichVu)
        .Include(x => x.DatPhong).ThenInclude(dp => dp!.Phong)
        .Where(x => x.DatPhong!.KH_ID == userId.Value).ToList();

        return View(list);
    }
    public IActionResult SuaThongTin()
    {
        var userId = HttpContext.Session.GetInt32("UserID");

        if (userId == null)
            return RedirectToAction("DangNhap");

        var user = _context.KhachHangs.FirstOrDefault(x => x.KH_ID == userId);

        return View(user);
    }

    [HttpPost]
    public IActionResult SuaThongTin(tblKhachHang model, IFormFile? avatarFile)
    {
        var user = _context.KhachHangs
            .FirstOrDefault(x => x.KH_ID == model.KH_ID);

        if (user == null)
            return RedirectToAction("DangNhap");

        user.KH_TenKhach = model.KH_TenKhach;
        user.KH_Email = model.KH_Email;
        user.KH_DienThoai = model.KH_DienThoai;

        if (avatarFile != null && avatarFile.Length > 0)
        {
            TempData["Success"] = avatarFile.FileName;

            string fileName = Guid.NewGuid().ToString()
                              + Path.GetExtension(avatarFile.FileName);

            string path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/img",
                fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                avatarFile.CopyTo(stream);
            }

            user.KH_HinhAnh = fileName;
        }

        _context.SaveChanges();

        TempData["Success"] = "Cập nhật thành công!";
        return RedirectToAction("TrangCaNhan");
    }

    [HttpPost]
    public IActionResult DoiMatKhau(string oldPass, string newPass)
    {
        var userId = HttpContext.Session.GetInt32("UserID");

        var user = _context.KhachHangs.FirstOrDefault(x => x.KH_ID == userId);

        if (user == null)
            return RedirectToAction("DangNhap");

        if (user.KH_MatKhau != oldPass)
        {
            TempData["Error"] = "Mật khẩu cũ không đúng!";
            return RedirectToAction("TrangCaNhan");
        }

        user.KH_MatKhau = newPass;
        _context.SaveChanges();

        TempData["Success"] = "Đổi mật khẩu thành công!";
        return RedirectToAction("TrangCaNhan");
    }

}