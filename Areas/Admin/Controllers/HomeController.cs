using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAn.Data;

namespace DoAn.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.TongKhachHang = _context.KhachHangs.Count();
            ViewBag.TongPhong = _context.Phongs.Count();
            ViewBag.TongDatPhong = _context.DatPhongs.Count();
            ViewBag.TongSuDungDichVu = _context.SuDungDichVus.Count();
            ViewBag.TongHoaDon = _context.HoaDons.Count();

            ViewBag.TongDoanhThu = _context.HoaDons
                .Where(x => x.HD_TrangThai == "Đã thanh toán")
                .Sum(x => (decimal?)x.HD_TongTien) ?? 0;

            ViewBag.RecentBookings = _context.DatPhongs
                .Include(x => x.KhachHang)
                .Include(x => x.Phong)
                .OrderByDescending(x => x.DP_ID)
                .Take(5)
                .ToList();

            ViewBag.RecentInvoices = _context.HoaDons
                .Include(x => x.DatPhong)
                    .ThenInclude(dp => dp!.KhachHang)
                .OrderByDescending(x => x.HD_ID)
                .Take(5)
                .ToList();

            ViewBag.ThongBao = _context.ThongBaos
                .OrderByDescending(x => x.TB_ThoiGian)
                .Take(5)
                .ToList();

            return View();
        }
    }
}