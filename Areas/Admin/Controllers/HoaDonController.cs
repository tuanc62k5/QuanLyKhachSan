using DoAn.Data;
using DoAn.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HoaDonController : Controller
    {
        private readonly AppDbContext _context;

        public HoaDonController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? keyword, string? trangThai, string? phuongThuc)
        {
            var query = _context.HoaDons.Include(x => x.DatPhong).ThenInclude(dp => dp!.Phong)
            .Include(x => x.DatPhong).ThenInclude(dp => dp!.KhachHang).AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x =>
                    x.HD_ID.ToString().Contains(keyword) ||
                    (x.DatPhong != null &&
                     x.DatPhong.KhachHang != null &&
                     x.DatPhong.KhachHang.KH_TenKhach.Contains(keyword)));
            }

            if (!string.IsNullOrEmpty(trangThai))
            {
                query = query.Where(x => x.HD_TrangThai == trangThai);
            }

            if (!string.IsNullOrEmpty(phuongThuc))
            {
                query = query.Where(x => x.HD_PhuongThuc == phuongThuc);
            }

            var hoaDonList = query.OrderBy(x => x.HD_ID).ToList();

            ViewBag.Keyword = keyword;
            ViewBag.TrangThai = trangThai;
            ViewBag.PhuongThuc = phuongThuc;

            ViewBag.ThongBao = _context.ThongBaos.OrderByDescending(x => x.TB_ThoiGian).Take(5).ToList();

            return View(hoaDonList);
        }

        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var hd = _context.HoaDons.Include(x => x.DatPhong).ThenInclude(dp => dp!.Phong).Include(x => x.DatPhong)
            .ThenInclude(dp => dp!.KhachHang).FirstOrDefault(x => x.HD_ID == id);

            if (hd == null)
                return NotFound();

            ViewBag.ThongBao = _context.ThongBaos.OrderByDescending(x => x.TB_ThoiGian).Take(5).ToList();

            return View(hd);
        }

        public IActionResult Create()
        {
            LoadData();

            return View();
        }

        [HttpPost]
        public IActionResult Create(tblHoaDon hd)
        {
            var datPhong = _context.DatPhongs.Include(x => x.Phong).FirstOrDefault(x => x.DP_ID == hd.DP_ID);

            if (datPhong == null)
                ModelState.AddModelError("DP_ID", "Vui lòng chọn đặt phòng!");

            if (ModelState.IsValid && datPhong != null)
            {
                hd.HD_TienPhong = datPhong.DP_TongTien;

                hd.HD_TienDichVu = _context.SuDungDichVus
                .Where(x => x.DP_ID == hd.DP_ID && x.SDDV_TrangThai == "Đã sử dụng")
                .Sum(x => (decimal?)x.SDDV_ThanhTien) ?? 0;

                hd.HD_TongTien = hd.HD_TienPhong + hd.HD_TienDichVu;

                hd.HD_NgayLap = DateTime.Now;

                _context.HoaDons.Add(hd);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            LoadData();
            return View(hd);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var hd = _context.HoaDons.FirstOrDefault(x => x.HD_ID == id);

            if (hd == null)
                return NotFound();

            LoadData(hd.DP_ID);

            return View(hd);
        }

        [HttpPost]
        public IActionResult Edit(tblHoaDon hd)
        {
            var datPhong = _context.DatPhongs.FirstOrDefault(x => x.DP_ID == hd.DP_ID);

            if (datPhong == null)
                ModelState.AddModelError("DP_ID", "Vui lòng chọn đặt phòng!");

            if (ModelState.IsValid && datPhong != null)
            {
                hd.HD_TienPhong = datPhong.DP_TongTien;

                hd.HD_TienDichVu = _context.SuDungDichVus
                .Where(x => x.DP_ID == hd.DP_ID && x.SDDV_TrangThai == "Đã sử dụng")
                .Sum(x => (decimal?)x.SDDV_ThanhTien) ?? 0;

                hd.HD_TongTien = hd.HD_TienPhong + hd.HD_TienDichVu;

                _context.HoaDons.Update(hd);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            LoadData(hd.DP_ID);

            return View(hd);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var hd = _context.HoaDons.Include(x => x.DatPhong).ThenInclude(dp => dp!.Phong).Include(x => x.DatPhong)
            .ThenInclude(dp => dp!.KhachHang).FirstOrDefault(x => x.HD_ID == id);

            if (hd == null)
                return NotFound();

            ViewBag.ThongBao = _context.ThongBaos.OrderByDescending(x => x.TB_ThoiGian).Take(5).ToList();

            return View(hd);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var hd = _context.HoaDons.Find(id);

            if (hd == null)
                return NotFound();

            _context.HoaDons.Remove(hd);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        private void LoadData(int? selectedDP = null)
        {
            ViewBag.DatPhongList = new SelectList(
                _context.DatPhongs.Include(x => x.Phong).Include(x => x.KhachHang)
                .Where(x => x.Phong != null && x.KhachHang != null).ToList().Select(x => new
                {
                    DP_ID = x.DP_ID,
                    Display = $"#{x.DP_ID} - {x.KhachHang!.KH_TenKhach} - {x.Phong!.P_TenPhong}"
                }), "DP_ID", "Display", selectedDP
            );

            ViewBag.ThongBao = _context.ThongBaos.OrderByDescending(x => x.TB_ThoiGian).Take(5).ToList();
        }

        public IActionResult Print(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var hd = _context.HoaDons.Include(x => x.DatPhong).ThenInclude(dp => dp!.Phong).Include(x => x.DatPhong)
            .ThenInclude(dp => dp!.KhachHang).FirstOrDefault(x => x.HD_ID == id);

            if (hd == null)
                return NotFound();

            var dichVuList = _context.SuDungDichVus.Include(x => x.DichVu).Where(x => x.DP_ID == hd.DP_ID && x.SDDV_TrangThai == "Đã sử dụng").ToList();

            ViewBag.DichVuList = dichVuList;

            return View(hd);
        }
    }
}