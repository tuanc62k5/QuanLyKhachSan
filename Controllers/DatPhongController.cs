using Microsoft.AspNetCore.Mvc;
using DoAn.Models;
using DoAn.Data;
using Microsoft.EntityFrameworkCore;
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

        if (phong == null)
            return NotFound();

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
            ModelState.AddModelError("", "Thời gian đặt phòng không hợp lệ");
            ViewBag.Phong = phong;
            return View(model);
        }

        if (model.DP_NgayNhan < DateTime.Now)
        {
            TempData["Error"] = "Không được đặt phòng trong thời gian đã qua!";

            return Redirect($"/Phong-{model.P_ID}.html");
        }

        model.KH_ID = userId.Value;

        model.DP_TongTien = (decimal)soGio * phong.P_GiaPhong;

        model.DP_NgayTao = DateTime.Now;

        if (string.IsNullOrEmpty(model.DP_TrangThai))
        {
            model.DP_TrangThai = "Chờ duyệt";
        }

        _context.DatPhongs.Add(model);

        phong.P_TrangThai = false;

        _context.ThongBaos.Add(new tblThongBao
        {
            TB_NoiDung = "Khách hàng " + user.KH_TenKhach + " vừa đặt phòng " + phong.P_TenPhong,
            TB_ThoiGian = DateTime.Now,
            TB_TrangThai = false
        });

        _context.SaveChanges();

        TempData["DatPhongSuccess"] = "🎉 Đặt phòng thành công!";

        return RedirectToAction("DanhSach", "Phong");
    }

    public IActionResult LichSu()
    {
        var userId = HttpContext.Session.GetInt32("UserID");

        if (userId == null)
            return RedirectToAction("DangNhap", "TaiKhoan");

        var lichSu = _context.DatPhongs.Include(x => x.Phong).Where(x => x.KH_ID == userId.Value)
        .OrderByDescending(x => x.DP_NgayTao).ToList();

        return View("~/Views/TaiKhoan/LichSu.cshtml", lichSu);
    }

    [HttpPost]
    public IActionResult XoaDatPhong(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserID");

        if (userId == null)
            return RedirectToAction("DangNhap", "TaiKhoan");

        var dp = _context.DatPhongs.FirstOrDefault(x => x.DP_ID == id && x.KH_ID == userId.Value);

        if (dp != null)
        {
            var phong = _context.Phongs.FirstOrDefault(p => p.P_ID == dp.P_ID);

            if (phong != null)
            {
                phong.P_TrangThai = true;
            }

            _context.DatPhongs.Remove(dp);
            _context.SaveChanges();
        }

        TempData["Success"] = "Đã hủy đặt phòng!";

        return RedirectToAction("LichSu");
    }
}