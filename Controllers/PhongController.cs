using Microsoft.AspNetCore.Mvc;
using DoAn.Models;
using DoAn.Data;
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

        // chỉ lấy phòng đang hoạt động
        query = query.Where(p => p.P_TrangThai);

        // lọc số người
        if (SoNguoi.HasValue && SoNguoi > 0)
            query = query.Where(p => p.P_SucChua >= SoNguoi.Value);

        // lọc giá
        if (GiaMin.HasValue)
            query = query.Where(p => p.P_GiaPhong >= GiaMin.Value);

        if (GiaMax.HasValue)
            query = query.Where(p => p.P_GiaPhong <= GiaMax.Value);

        // sắp xếp
        if (SapXep == "GiaTang")
            query = query.OrderBy(p => p.P_GiaPhong);
        else if (SapXep == "GiaGiam")
            query = query.OrderByDescending(p => p.P_GiaPhong);

        var phongs = query.ToList();

        // giữ lại giá trị filter
        ViewBag.SoNguoi = SoNguoi;
        ViewBag.GiaMin = GiaMin;
        ViewBag.GiaMax = GiaMax;
        ViewBag.SapXep = SapXep;

        return View(phongs);
    }
    [HttpPost]
    public IActionResult DatPhong(int P_ID, string KH_TenKhach, string KH_Email, string KH_DienThoai, DateTime DP_NgayNhan, DateTime DP_NgayTra, int DP_SoNguoi)
    {
        var phong = _context.Phongs.FirstOrDefault(p => p.P_ID == P_ID);

        if (phong == null)
            return NotFound();

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
            P_ID = P_ID,
            KH_TenKhach = KH_TenKhach,
            KH_Email = KH_Email,
            KH_DienThoai = KH_DienThoai,
            DP_NgayNhan = DP_NgayNhan,
            DP_NgayTra = DP_NgayTra,
            DP_SoNguoi = DP_SoNguoi,
            DP_TongTien = tongTien
        };

        _context.DatPhongs.Add(datPhong);
        _context.SaveChanges();

        TempData["DatPhongSuccess"] = "Đặt phòng thành công!";
        return RedirectToAction("DanhSach");
    }
}