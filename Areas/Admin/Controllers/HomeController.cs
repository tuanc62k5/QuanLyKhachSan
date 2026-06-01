using DoAn.Data;
using DoAn.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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

            if (!Functions.IsLogin())
                return RedirectToAction("DangNhap", "TaiKhoan");

            ViewBag.TuNgay = tuNgay;
            ViewBag.DenNgay = denNgay;

            var datPhongs = _context.DatPhongs.Include(x => x.KhachHang).Include(x => x.Phong).AsQueryable();
            var hoaDons = _context.HoaDons.Where(x => x.HD_TrangThai == "Đã thanh toán").AsQueryable();
            var chartLabels = new List<string>();
            var chartTong = new List<decimal>();
            var chartPhong = new List<decimal>();
            var chartDichVu = new List<decimal>();

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

            var minDate = hoaDons.Any() ? hoaDons.Min(x => x.HD_NgayLap) : DateTime.Today;

            var maxDate = hoaDons.Any() ? hoaDons.Max(x => x.HD_NgayLap) : DateTime.Today;

            var startDate = tuNgay ?? minDate;
            var endDate = denNgay ?? maxDate;
            var soNgay = (endDate.Date - startDate.Date).Days + 1;

            if (soNgay < 7)
            {
                var data = hoaDons.AsEnumerable().GroupBy(x => x.HD_NgayLap.Date).OrderBy(g => g.Key).Select(g => new
                {
                    Label = g.Key.ToString("dd/MM"),
                    Tong = g.Sum(x => x.HD_TongTien),
                    Phong = g.Sum(x => x.HD_TienPhong),
                    DichVu = g.Sum(x => x.HD_TienDichVu)
                }).ToList();

                chartLabels = data.Select(x => x.Label).ToList();
                chartTong = data.Select(x => x.Tong).ToList();
                chartPhong = data.Select(x => x.Phong).ToList();
                chartDichVu = data.Select(x => x.DichVu).ToList();

                ViewBag.ChartType = "ngày";
            }

            else if (soNgay < 49)
            {
                var data = hoaDons.AsEnumerable().GroupBy(x => (int)((x.HD_NgayLap.Date - startDate.Date).TotalDays / 7))
                .Select(g => new
                {
                    Label = $"Tuần {g.Key + 1}",
                    Tong = g.Sum(x => x.HD_TongTien),
                    Phong = g.Sum(x => x.HD_TienPhong),
                    DichVu = g.Sum(x => x.HD_TienDichVu)
                }).ToList();

                chartLabels = data.Select(x => x.Label).ToList();
                chartTong = data.Select(x => x.Tong).ToList();
                chartPhong = data.Select(x => x.Phong).ToList();
                chartDichVu = data.Select(x => x.DichVu).ToList();

                ViewBag.ChartType = "tuần";
            }

            else
            {
                var data = hoaDons.AsEnumerable().GroupBy(x => new
                {
                    x.HD_NgayLap.Year,
                    x.HD_NgayLap.Month
                }).Select(g => new
                {
                    Label = $"{g.Key.Month:00}/{g.Key.Year}",
                    Tong = g.Sum(x => x.HD_TongTien),
                    Phong = g.Sum(x => x.HD_TienPhong),
                    DichVu = g.Sum(x => x.HD_TienDichVu)
                }).OrderBy(x => x.Label).ToList();

                chartLabels = data.Select(x => x.Label).ToList();
                chartTong = data.Select(x => x.Tong).ToList();
                chartPhong = data.Select(x => x.Phong).ToList();
                chartDichVu = data.Select(x => x.DichVu).ToList();

                ViewBag.ChartType = "tháng";
            }

            ViewBag.ChartLabels = chartLabels;

            ViewBag.ChartTong = chartTong;

            ViewBag.ChartPhong = chartPhong;

            ViewBag.ChartDichVu = chartDichVu;

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
            .OrderByDescending(x => x.HD_ID).Take(3).ToList();

            return View();
        }

        public IActionResult Logout()
        {
            Functions._KH_ID = 0;
            Functions._TenKhach = "";
            Functions._Email = "";
            Functions._VaiTro = "";

            return RedirectToAction("Index", "Home");
        }

    }
}