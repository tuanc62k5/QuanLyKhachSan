using Microsoft.AspNetCore.Mvc;
using DoAn.Models;
using System.Linq;

public class PhongController : Controller
{
    private readonly AppDbContext _context;

    public PhongController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult DanhSach(int SoNguoi = 1, decimal? GiaMin = null, decimal? GiaMax = null)
    {
        var query = _context.Phongs
            .Where(p => p.P_TrangThai && p.P_SucChua >= SoNguoi);

        if (GiaMin.HasValue)
            query = query.Where(p => p.P_GiaPhong >= GiaMin.Value);

        if (GiaMax.HasValue)
            query = query.Where(p => p.P_GiaPhong <= GiaMax.Value);

        var phongs = query
            .OrderBy(p => p.P_GiaPhong)
            .ToList();

        ViewBag.SoNguoi = SoNguoi;
        ViewBag.GiaMin = GiaMin;
        ViewBag.GiaMax = GiaMax;

        return View(phongs);
    }
    [HttpPost]
    public IActionResult DatPhong(int P_ID, string KH_TenKhach, string KH_Email, string KH_DienThoai, DateTime DP_NgayNhan, DateTime DP_NgayTra, int DP_SoNguoi)
    {
        var phong = _context.Phongs.FirstOrDefault(p => p.P_ID == P_ID);

        if (phong == null)
            return NotFound();

        int SoNgay = (DP_NgayTra - DP_NgayNhan).Days;

        if (SoNgay <= 0)
        {
            TempData["Error"] = "Ngày trả phải lớn hơn ngày nhận";
            return RedirectToAction("Details", new { id = P_ID });
        }

        decimal tongTien = SoNgay * phong.P_GiaPhong;
        decimal tienCoc = tongTien * 0.3m;

        var datPhong = new tblDatPhong
        {
            P_ID = P_ID,
            KH_TenKhach = KH_TenKhach,
            KH_Email = KH_Email,
            KH_DienThoai = KH_DienThoai,
            DP_NgayNhan = DP_NgayNhan,
            DP_NgayTra = DP_NgayTra,
            DP_SoNguoi = DP_SoNguoi,
            DP_TongTien = tongTien,
            DP_TienCoc = tienCoc
        };

        _context.DatPhongs.Add(datPhong);
        _context.SaveChanges();

        TempData["DatPhongSuccess"] = "Đặt phòng thành công!";
        return RedirectToAction("DanhSach");
    }
}