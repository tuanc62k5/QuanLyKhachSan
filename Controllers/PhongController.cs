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

    public IActionResult DanhSach(int NguoiLon = 1, int TreEm = 0, decimal? GiaMin = null, decimal? GiaMax = null)
    {
        int tongNguoi = NguoiLon + TreEm;

        var query = _context.Phongs
            .Where(p => p.P_TrangThai && p.P_SucChua >= tongNguoi);

        if (GiaMin.HasValue)
            query = query.Where(p => p.P_GiaPhong >= GiaMin);

        if (GiaMax.HasValue)
            query = query.Where(p => p.P_GiaPhong <= GiaMax);

        var phongs = query
            .OrderBy(p => p.P_GiaPhong)
            .ToList();

        ViewBag.NguoiLon = NguoiLon;
        ViewBag.TreEm = TreEm;
        ViewBag.GiaMin = GiaMin;
        ViewBag.GiaMax = GiaMax;

        return View(phongs);
    }
}