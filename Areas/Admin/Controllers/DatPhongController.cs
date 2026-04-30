using DoAn.Models;
using DoAn.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DatPhongController : Controller
    {
        private readonly AppDbContext _context;

        public DatPhongController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string keyword, string loaiPhong, string trangThai)
        {
            var query = _context.DatPhongs
                .Include(dp => dp.Phong)
                .Include(dp => dp.KhachHang)
                .AsQueryable();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(dp =>
                    (dp.KhachHang != null && dp.KhachHang.KH_TenKhach.Contains(keyword)) ||
                    (dp.Phong != null && dp.Phong.P_TenPhong.Contains(keyword)) ||
                    dp.DP_ID.ToString().Contains(keyword)
                );
            }

            // Lọc loại phòng
            if (!string.IsNullOrEmpty(loaiPhong))
            {
                query = query.Where(dp =>
                    dp.Phong != null &&
                    dp.Phong.P_LoaiPhong == loaiPhong
                );
            }

            // Lọc trạng thái
            if (!string.IsNullOrEmpty(trangThai))
            {
                query = query.Where(dp =>
                    dp.DP_TrangThai == trangThai
                );
            }

            var datPhongList = query
                .OrderByDescending(dp => dp.DP_NgayTao)
                .ToList();

            // Danh sách loại phòng
            ViewBag.LoaiPhongList = _context.Phongs
                .Where(p => p.P_LoaiPhong != null)
                .Select(p => p.P_LoaiPhong)
                .Distinct()
                .ToList();

            // Thông báo
            ViewBag.ThongBao = _context.ThongBaos
                .OrderByDescending(tb => tb.TB_ThoiGian)
                .Take(5)
                .ToList();

            ViewBag.Keyword = keyword;
            ViewBag.LoaiPhong = loaiPhong;
            ViewBag.TrangThai = trangThai;

            return View(datPhongList);
        }

        // GET: Admin/DatPhong/Create
        public IActionResult Create()
        {
            var phongs = _context.Phongs
                .Where(p => p.P_TrangThai)
                .ToList();

            // Dropdown loại phòng (lọc)
            ViewBag.LoaiPhongList = phongs
                .Select(p => p.P_LoaiPhong)
                .Distinct()
                .ToList();

            // Dropdown phòng
            ViewBag.PhongList = phongs;

            // Dropdown khách hàng
            ViewBag.KhachHangList = _context.KhachHangs
                .Select(k => new SelectListItem
                {
                    Value = k.KH_ID.ToString(),
                    Text = k.KH_TenKhach
                })
                .ToList();

            return View();
        }

        // POST: Admin/DatPhong/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(tblDatPhong dp)
        {
            var phong = _context.Phongs
                .FirstOrDefault(p => p.P_ID == dp.P_ID);

            var khach = _context.KhachHangs
                .FirstOrDefault(k => k.KH_ID == dp.KH_ID);

            // Validate phòng
            if (phong == null)
            {
                ModelState.AddModelError("P_ID", "Vui lòng chọn phòng!");
            }

            // Validate khách hàng
            if (khach == null)
            {
                ModelState.AddModelError("KH_ID", "Vui lòng chọn khách hàng!");
            }

            // Validate thời gian
            if (dp.DP_NgayTra <= dp.DP_NgayNhan)
            {
                ModelState.AddModelError("", "Ngày trả phải lớn hơn ngày nhận!");
            }

            if (ModelState.IsValid && phong != null && khach != null)
            {
                // Tính theo giờ
                double soGio = Math.Ceiling((dp.DP_NgayTra - dp.DP_NgayNhan).TotalHours);

                if (soGio <= 0)
                {
                    soGio = 1;
                }

                dp.DP_TongTien = phong.P_GiaPhong * (decimal)soGio;
                dp.DP_NgayTao = DateTime.Now;

                _context.DatPhongs.Add(dp);

                // Thông báo
                _context.ThongBaos.Add(new ThongBao
                {
                    TB_NoiDung = "Đã tạo đơn đặt phòng mới",
                    TB_ThoiGian = DateTime.Now,
                    TB_TrangThai = false
                });

                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            // Load lại dữ liệu nếu lỗi
            var phongs = _context.Phongs
                .Where(p => p.P_TrangThai)
                .ToList();

            ViewBag.LoaiPhongList = phongs
                .Select(p => p.P_LoaiPhong)
                .Distinct()
                .ToList();

            ViewBag.PhongList = phongs;

            ViewBag.KhachHangList = _context.KhachHangs
                .Select(k => new SelectListItem
                {
                    Value = k.KH_ID.ToString(),
                    Text = k.KH_TenKhach
                })
                .ToList();

            return View(dp);
        }
    }
}