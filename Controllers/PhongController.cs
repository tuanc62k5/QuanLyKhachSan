using Microsoft.AspNetCore.Mvc;
using DoAn.Models;
using DoAn.Data;
using System;
using System.Linq;

public class PhongController : Controller
{
    private readonly AppDbContext _context;

    public PhongController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult DanhSach(int? SoNguoi, decimal? GiaMin, decimal? GiaMax, string SapXep)
    {
        var query = _context.Phongs.AsQueryable();

        query = query.Where(p => p.P_TrangThai);

        if (SoNguoi.HasValue && SoNguoi > 0)
            query = query.Where(p => p.P_SucChua >= SoNguoi.Value);

        if (GiaMin.HasValue)
            query = query.Where(p => p.P_GiaPhong >= GiaMin.Value);

        if (GiaMax.HasValue)
            query = query.Where(p => p.P_GiaPhong <= GiaMax.Value);

        if (SapXep == "GiaTang")
            query = query.OrderBy(p => p.P_GiaPhong);
        else if (SapXep == "GiaGiam")
            query = query.OrderByDescending(p => p.P_GiaPhong);

        var phongs = query.ToList();

        ViewBag.SoNguoi = SoNguoi;
        ViewBag.GiaMin = GiaMin;
        ViewBag.GiaMax = GiaMax;
        ViewBag.SapXep = SapXep;

        return View(phongs);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DatPhong(int P_ID, DateTime DP_NgayNhan, DateTime DP_NgayTra, int DP_SoNguoi)
    {
        var userId = HttpContext.Session.GetInt32("UserID");

        if (userId == null)
            return RedirectToAction("DangNhap", "TaiKhoan");

        var phong = _context.Phongs
            .FirstOrDefault(p => p.P_ID == P_ID);

        if (phong == null)
            return NotFound();

        var khachHang = _context.KhachHangs
            .FirstOrDefault(k => k.KH_ID == userId.Value);

        if (khachHang == null)
            return RedirectToAction("DangNhap", "TaiKhoan");

        var soGio = (DP_NgayTra - DP_NgayNhan).TotalHours;

        if (soGio <= 0)
        {
            TempData["Error"] = "Giờ trả phải lớn hơn giờ nhận";
            return RedirectToAction("Details", new { id = P_ID });
        }

        soGio = Math.Ceiling(soGio);

        decimal tongTien = (decimal)soGio * phong.P_GiaPhong;

        var datPhong = new tblDatPhong
        {
            KH_ID = userId.Value,
            P_ID = P_ID,
            DP_NgayNhan = DP_NgayNhan,
            DP_NgayTra = DP_NgayTra,
            DP_SoNguoi = DP_SoNguoi,
            DP_TongTien = tongTien,
            DP_NgayTao = DateTime.Now,
            DP_TrangThai = "Chờ duyệt"
        };

        _context.DatPhongs.Add(datPhong);

        _context.ThongBaos.Add(new ThongBao
        {
            TB_NoiDung = "Khách hàng " + khachHang.KH_TenKhach + " vừa đặt phòng " + phong.P_TenPhong,
            TB_ThoiGian = DateTime.Now,
            TB_TrangThai = false
        });

        _context.SaveChanges();

        TempData["DatPhongSuccess"] = "Đặt phòng thành công!";

        return RedirectToAction("DanhSach");
    }
}