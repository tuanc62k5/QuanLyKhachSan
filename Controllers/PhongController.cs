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

    [Route("Phongs")]
    public IActionResult DanhSach(int NguoiLon = 1, int TreEm = 0)
    {
        int tongNguoi = NguoiLon + TreEm;

        var phongs = _context.Phongs
            .Where(p => p.P_TrangThai && p.P_SucChua >= tongNguoi)
            .OrderBy(p => p.P_GiaPhong)
            .ToList();

        ViewBag.NguoiLon = NguoiLon;
        ViewBag.TreEm = TreEm;

        return View(phongs);
    }
}