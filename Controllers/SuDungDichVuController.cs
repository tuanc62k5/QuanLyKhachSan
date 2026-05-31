using Microsoft.AspNetCore.Mvc;
using DoAn.Models;
using DoAn.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class SuDungDichVuController : Controller
{
    private readonly AppDbContext _context;

    public SuDungDichVuController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult DichVuDaDat()
    {
        var userId = HttpContext.Session.GetInt32("UserID");

        if (userId == null)
            return RedirectToAction("DangNhap", "TaiKhoan");

        var lichSu = _context.SuDungDichVus.Include(x => x.DichVu).Include(x => x.DatPhong)
        .ThenInclude(dp => dp!.Phong).Where(x => x.DatPhong != null && x.DatPhong.KH_ID == userId.Value)
        .OrderByDescending(x => x.SDDV_NgaySuDung).ToList();

        return View(lichSu);
    }

    public IActionResult DatDichVu(int dvId, int dpId)
    {
        var dv = _context.DichVus.FirstOrDefault(x => x.DV_ID == dvId);

        if (dv == null)
            return NotFound();

        var datPhong = _context.DatPhongs.FirstOrDefault(x => x.DP_ID == dpId);

        if (datPhong == null)
            return NotFound();

        if (datPhong.DP_TrangThai == "Đã hủy" || datPhong.DP_TrangThai == "Đã trả phòng")
        {
            TempData["Error"] = "Không thể đặt dịch vụ cho phòng này!";
            return RedirectToAction("LichSu", "TaiKhoan");
        }

        string trangThaiDV = "Chờ sử dụng";

        if (datPhong.DP_TrangThai == "Đang ở")
        {
            trangThaiDV = "Đang sử dụng";
        }

        var model = new tblSuDungDichVu
        {
            DV_ID = dvId,
            DP_ID = dpId,
            SDDV_SoLuong = 1,
            SDDV_ThanhTien = dv.DV_GiaTien,
            SDDV_NgaySuDung = DateTime.Now,
            SDDV_TrangThai = trangThaiDV
        };

        _context.SuDungDichVus.Add(model);
        _context.SaveChanges();

        TempData["Success"] = "Đặt dịch vụ thành công!";

        return RedirectToAction("DichVuDaDat", "TaiKhoan");
    }

    [HttpPost]
    public IActionResult XoaDichVu(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserID");

        if (userId == null)
            return RedirectToAction("DangNhap", "TaiKhoan");

        var dv = _context.SuDungDichVus.Include(x => x.DatPhong)
        .FirstOrDefault(x => x.SDDV_ID == id && x.DatPhong!.KH_ID == userId.Value);

        if (dv != null)
        {
            _context.SuDungDichVus.Remove(dv);
            _context.SaveChanges();
        }

        TempData["Success"] = "Đã hủy dịch vụ!";

        return RedirectToAction("DichVuDaDat");
    }
}