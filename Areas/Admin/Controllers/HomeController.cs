using DoAn.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public IActionResult Index(DateTime? tuNgay, DateTime? denNgay)
        {
            ViewBag.TuNgay = tuNgay;
            ViewBag.DenNgay = denNgay;

            var datPhongs = _context.DatPhongs.Include(x => x.KhachHang).Include(x => x.Phong).AsQueryable();
            var hoaDons = _context.HoaDons.Where(x => x.HD_TrangThai == "Đã thanh toán").AsQueryable();

            if (tuNgay.HasValue)
            {
                datPhongs = datPhongs.Where(x => x.DP_NgayTao >= tuNgay.Value);
                hoaDons = hoaDons.Where(x => x.HD_NgayLap >= tuNgay.Value);
            }

            if (denNgay.HasValue)
            {
                var den = denNgay.Value.AddDays(1);

                datPhongs = datPhongs.Where(x => x.DP_NgayTao < den);
                hoaDons = hoaDons.Where(x => x.HD_NgayLap < den);
            }

            ViewBag.TongDoanhThu = hoaDons.Sum(x => (decimal?)x.HD_TongTien) ?? 0;

            ViewBag.TongDatPhong = datPhongs.Count();

            ViewBag.KhachDangO = datPhongs.Count(x => x.DP_TrangThai == "Đang ở");

            ViewBag.PhongTrong = _context.Phongs.Count(x => x.P_TrangThai == true);

            ViewBag.PhongDaDat = _context.Phongs.Count(x => x.P_TrangThai == false);

            ViewBag.DatPhongChoDuyet = datPhongs.Count(x => x.DP_TrangThai == "Chờ duyệt");

            ViewBag.DatPhongDaDuyet = datPhongs.Count(x => x.DP_TrangThai == "Đã duyệt");

            ViewBag.TongKhachHang = _context.KhachHangs.Count();

            ViewBag.TongPhong = _context.Phongs.Count();

            ViewBag.TongHoaDon = _context.HoaDons.Count();

            ViewBag.TongSuDungDichVu = _context.SuDungDichVus.Count();

            ViewBag.TopPhong = _context.DatPhongs.Include(x => x.Phong).AsEnumerable().GroupBy(x => x.Phong?.P_TenPhong)
            .Select(x => new { TenPhong = x.Key, SoLuot = x.Count() }).OrderByDescending(x => x.SoLuot).Take(5).ToList();

            ViewBag.TopDichVu = _context.SuDungDichVus.Include(x => x.DichVu).AsEnumerable().GroupBy(x => x.DichVu?.DV_TenDichVu)
            .Select(x => new { TenDichVu = x.Key, SoLanSuDung = x.Count() }).OrderByDescending(x => x.SoLanSuDung).Take(5).ToList();

            ViewBag.RecentBookings = datPhongs.OrderByDescending(x => x.DP_ID).Take(5).ToList();

            ViewBag.RecentInvoices = _context.HoaDons.Include(x => x.DatPhong).ThenInclude(dp => dp!.KhachHang)
            .OrderByDescending(x => x.HD_ID).Take(5).ToList();

            return View();
        }
    }
}