using DoAn.Data;
using DoAn.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SuDungDichVuController : Controller
    {
        private readonly AppDbContext _context;

        public SuDungDichVuController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? keyword, int? phongId, int? khachHangId, int? dichVuId, string? trangThai)
        {
            var query = _context.SuDungDichVus.Include(x => x.DatPhong).ThenInclude(dp => dp!.Phong).Include(x => x.DatPhong)
                .ThenInclude(dp => dp!.KhachHang).Include(x => x.DichVu).AsQueryable();

            if (phongId.HasValue)
            {
                query = query.Where(x => x.DatPhong != null && x.DatPhong.P_ID == phongId.Value);
            }

            if (khachHangId.HasValue)
            {
                query = query.Where(x => x.DatPhong != null && x.DatPhong.KH_ID == khachHangId.Value);
            }

            if (dichVuId.HasValue)
            {
                query = query.Where(x => x.DV_ID == dichVuId.Value);
            }

            if (!string.IsNullOrEmpty(trangThai))
            {
                query = query.Where(x => x.SDDV_TrangThai == trangThai);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => (x.DatPhong != null && x.DatPhong.KhachHang != null && x.DatPhong.KhachHang.KH_TenKhach
                .Contains(keyword)) || x.SDDV_ID.ToString().Contains(keyword));
            }

            var list = query.OrderBy(x => x.SDDV_ID).ToList();

            ViewBag.PhongList = new SelectList(_context.DatPhongs.Include(x => x.Phong).Where(x => x.Phong != null)
            .Select(x => x.Phong).Distinct().ToList(), "P_ID", "P_TenPhong", phongId);

            var khachQuery = _context.DatPhongs.Include(x => x.KhachHang).AsQueryable();

            if (phongId.HasValue)
            {
                khachQuery = khachQuery.Where(x => x.P_ID == phongId.Value);
            }

            ViewBag.KhachHangList = new SelectList(khachQuery.Where(x => x.KhachHang != null)
            .Select(x => x.KhachHang).Distinct().ToList(), "KH_ID", "KH_TenKhach", khachHangId);

            var dichVuQuery = _context.SuDungDichVus.Include(x => x.DatPhong).Include(x => x.DichVu).AsQueryable();

            if (phongId.HasValue)
            {
                dichVuQuery = dichVuQuery.Where(x => x.DatPhong != null && x.DatPhong.P_ID == phongId.Value);
            }

            if (khachHangId.HasValue)
            {
                dichVuQuery = dichVuQuery.Where(x => x.DatPhong != null && x.DatPhong.KH_ID == khachHangId.Value);
            }

            ViewBag.DichVuList = new SelectList(dichVuQuery.Where(x => x.DichVu != null)
            .Select(x => x.DichVu).Distinct().ToList(), "DV_ID", "DV_TenDichVu", dichVuId);

            ViewBag.Keyword = keyword;
            ViewBag.PhongId = phongId;
            ViewBag.KhachHangId = khachHangId;
            ViewBag.DichVuId = dichVuId;
            ViewBag.TrangThai = trangThai;

            ViewBag.ThongBao = _context.ThongBaos.OrderByDescending(x => x.TB_ThoiGian).Take(5).ToList();

            return View(list);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var sddv = _context.SuDungDichVus.Include(x => x.DatPhong).ThenInclude(dp => dp!.Phong).Include(x => x.DatPhong)
            .ThenInclude(dp => dp!.KhachHang).Include(x => x.DichVu).FirstOrDefault(x => x.SDDV_ID == id);

            if (sddv == null)
                return NotFound();

            ViewBag.ThongBao = _context.ThongBaos.OrderByDescending(x => x.TB_ThoiGian).Take(5).ToList();

            return View(sddv);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var delSuDungDichVu = _context.SuDungDichVus.Find(id);

            if (delSuDungDichVu == null)
                return NotFound();

            _context.SuDungDichVus.Remove(delSuDungDichVu);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Create()
        {
            ViewBag.PhongList = _context.DatPhongs.Include(dp => dp.Phong).Where(dp => dp.Phong != null)
            .Select(dp => new SelectListItem
            {
                Value = dp.Phong!.P_ID.ToString(),
                Text = dp.Phong.P_TenPhong
            }).Distinct().ToList();

            ViewBag.DatPhongList = _context.DatPhongs.Include(dp => dp.Phong).Include(dp => dp.KhachHang).Where(dp => dp.Phong != null && dp.KhachHang != null)
            .Select(dp => new
            {
                Value = dp.DP_ID,
                Text = dp.KhachHang!.KH_TenKhach + " - " + dp.Phong!.P_TenPhong,
                PhongId = dp.P_ID
            }).ToList();

            ViewBag.DichVuList = _context.DichVus.Where(dv => dv.DV_TrangThai == true)
            .Select(dv => new
            {
                Value = dv.DV_ID,
                Text = dv.DV_TenDichVu,
                GiaTien = dv.DV_GiaTien
            }).ToList();

            ViewBag.ThongBao = _context.ThongBaos.OrderByDescending(x => x.TB_ThoiGian).Take(5).ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Create(tblSuDungDichVu sddv)
        {
            var datPhong = _context.DatPhongs.Include(dp => dp.Phong).Include(dp => dp.KhachHang).FirstOrDefault(dp => dp.DP_ID == sddv.DP_ID);

            var dichVu = _context.DichVus.FirstOrDefault(dv => dv.DV_ID == sddv.DV_ID);

            if (datPhong == null)
                ModelState.AddModelError("DP_ID", "Vui lòng chọn khách hàng / phòng!");

            if (dichVu == null)
                ModelState.AddModelError("DV_ID", "Vui lòng chọn dịch vụ!");

            if (sddv.SDDV_SoLuong <= 0)
                ModelState.AddModelError("SDDV_SoLuong", "Số lượng phải lớn hơn 0!");

            if (ModelState.IsValid && datPhong != null && dichVu != null)
            {
                sddv.SDDV_ThanhTien = sddv.SDDV_SoLuong * dichVu.DV_GiaTien;

                if (sddv.SDDV_NgaySuDung == DateTime.MinValue)
                {
                    sddv.SDDV_NgaySuDung = DateTime.Now;
                }

                _context.SuDungDichVus.Add(sddv);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.PhongList = _context.DatPhongs.Include(dp => dp.Phong).Where(dp => dp.Phong != null)
            .Select(dp => new SelectListItem
            {
                Value = dp.Phong!.P_ID.ToString(),
                Text = dp.Phong.P_TenPhong
            }).Distinct().ToList();

            ViewBag.DatPhongList = _context.DatPhongs.Include(dp => dp.Phong).Include(dp => dp.KhachHang).Where(dp => dp.Phong != null && dp.KhachHang != null)
            .Select(dp => new
            {
                Value = dp.DP_ID,
                Text = dp.KhachHang!.KH_TenKhach + " - " + dp.Phong!.P_TenPhong,
                PhongId = dp.P_ID
            }).ToList();

            ViewBag.DichVuList = _context.DichVus.Where(dv => dv.DV_TrangThai == true)
            .Select(dv => new
            {
                Value = dv.DV_ID,
                Text = dv.DV_TenDichVu,
                GiaTien = dv.DV_GiaTien
            }).ToList();

            ViewBag.ThongBao = _context.ThongBaos.OrderByDescending(x => x.TB_ThoiGian).Take(5).ToList();

            return View(sddv);
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var sddv = _context.SuDungDichVus.FirstOrDefault(x => x.SDDV_ID == id);

            if (sddv == null)
                return NotFound();

            ViewBag.DatPhongList = new SelectList(_context.DatPhongs.Include(x => x.Phong).Include(x => x.KhachHang)
            .Where(x => x.Phong != null && x.KhachHang != null).ToList().Select(x => new
            {
                DP_ID = x.DP_ID,
                Display = $"{x.KhachHang!.KH_TenKhach} - {x.Phong!.P_TenPhong}"
            }), "DP_ID", "Display", sddv.DP_ID);

            ViewBag.DichVuList = new SelectList(_context.DichVus.Where(x => x.DV_TrangThai).ToList(), "DV_ID", "DV_TenDichVu", sddv.DV_ID);

            ViewBag.DichVuData = _context.DichVus.Where(x => x.DV_TrangThai).Select(x => new
            {
                x.DV_ID,
                x.DV_GiaTien
            }).ToList();

            ViewBag.ThongBao = _context.ThongBaos.OrderByDescending(x => x.TB_ThoiGian).Take(5).ToList();

            return View(sddv);
        }

        [HttpPost]
        public IActionResult Edit(tblSuDungDichVu sddv)
        {
            var datPhong = _context.DatPhongs.FirstOrDefault(x => x.DP_ID == sddv.DP_ID);

            var dichVu = _context.DichVus.FirstOrDefault(x => x.DV_ID == sddv.DV_ID);

            if (datPhong == null)
                ModelState.AddModelError("DP_ID", "Vui lòng chọn đặt phòng!");

            if (dichVu == null)
                ModelState.AddModelError("DV_ID", "Vui lòng chọn dịch vụ!");

            if (sddv.SDDV_SoLuong <= 0)
                ModelState.AddModelError("SDDV_SoLuong", "Số lượng phải lớn hơn 0!");

            if (ModelState.IsValid && dichVu != null)
            {
                sddv.SDDV_ThanhTien = sddv.SDDV_SoLuong * dichVu.DV_GiaTien;

                _context.SuDungDichVus.Update(sddv);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.DatPhongList = new SelectList(_context.DatPhongs.Include(x => x.Phong).Include(x => x.KhachHang)
            .Where(x => x.Phong != null && x.KhachHang != null).ToList().Select(x => new
            {
                DP_ID = x.DP_ID,
                Display = $"{x.KhachHang!.KH_TenKhach} - {x.Phong!.P_TenPhong}"
            }), "DP_ID", "Display", sddv.DP_ID);

            ViewBag.DichVuList = new SelectList(_context.DichVus.Where(x => x.DV_TrangThai).ToList(), "DV_ID", "DV_TenDichVu", sddv.DV_ID);

            ViewBag.DichVuData = _context.DichVus.Where(x => x.DV_TrangThai).Select(x => new
            {
                x.DV_ID,
                x.DV_GiaTien
            }).ToList();

            ViewBag.ThongBao = _context.ThongBaos.OrderByDescending(x => x.TB_ThoiGian).Take(5).ToList();

            return View(sddv);
        }

        private void LoadData()
        {
            ViewBag.DatPhongList = new SelectList(_context.DatPhongs.Include(x => x.Phong).Include(x => x.KhachHang).ToList()
            .Select(x => new
            {
                DP_ID = x.DP_ID,
                Display = $"#{x.DP_ID} - {x.KhachHang!.KH_TenKhach} - {x.Phong!.P_TenPhong}"
            }), "DP_ID", "Display");

            ViewBag.DichVuList = new SelectList(_context.DichVus.Where(x => x.DV_TrangThai).ToList(), "DV_ID", "DV_TenDichVu");

            ViewBag.ThongBao = _context.ThongBaos.OrderByDescending(x => x.TB_ThoiGian).Take(5).ToList();
        }
    }
}