using Microsoft.AspNetCore.Mvc;
using DoAn.Models;
using DoAn.Data;
using System;
using System.Linq;

public class DatPhongController : Controller
{
    private readonly AppDbContext _context;

    public DatPhongController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(int P_ID)
    {
        var phong = _context.Phongs.FirstOrDefault(p => p.P_ID == P_ID);
        ViewBag.Phong = phong;

        return View();
    }

    [HttpPost]
    public IActionResult Index(tblDatPhong model)
    {
        var userId = HttpContext.Session.GetInt32("UserID");

        if (userId == null)
            return RedirectToAction("DangNhap", "TaiKhoan");

        var phong = _context.Phongs.FirstOrDefault(p => p.P_ID == model.P_ID);
        if (phong == null)
            return NotFound();

        var user = _context.KhachHangs.FirstOrDefault(x => x.KH_ID == userId.Value);
        if (user == null)
            return RedirectToAction("DangNhap", "TaiKhoan");

        var soGio = Math.Ceiling((model.DP_NgayTra - model.DP_NgayNhan).TotalHours);

        if (soGio <= 0)
        {
            ModelState.AddModelError("", "Ngày không hợp lệ");
            ViewBag.Phong = phong;
            return View(model);
        }

        model.KH_ID = userId.Value;
        model.KH_TenKhach = user.KH_TenKhach ?? "";
        model.KH_Email = user.KH_Email ?? "";
        model.KH_DienThoai = user.KH_DienThoai ?? "";

        model.DP_TongTien = (decimal)soGio * phong.P_GiaPhong;
        model.DP_NgayTao = DateTime.Now;

        _context.DatPhongs.Add(model);
        _context.SaveChanges();

        TempData["DatPhongSuccess"] = "🎉 Đặt phòng thành công!";
        return RedirectToAction("DanhSach", "Phong");
    }
    [HttpPost]
    public IActionResult XoaDatPhong(int id)
    {
        var dp = _context.DatPhongs.FirstOrDefault(x => x.DP_ID == id);

        if (dp != null)
        {
            _context.DatPhongs.Remove(dp);
            _context.SaveChanges();
        }

        TempData["Success"] = "Đã hủy đặt phòng!";
        return RedirectToAction("LichSu");
    }
}