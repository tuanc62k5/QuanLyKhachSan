using DoAn.Models;
using DoAn.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PhongController : Controller
    {
        private readonly AppDbContext _context;

        public PhongController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string keyword, string loaiPhong, int? sucChua, bool? trangThai)
        {
            var query = _context.Phongs.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p =>
                    p.P_TenPhong.Contains(keyword) || p.P_LoaiPhong.Contains(keyword)
                );
            }

            if (!string.IsNullOrEmpty(loaiPhong))
            {
                query = query.Where(p => p.P_LoaiPhong == loaiPhong);
            }
            if (sucChua.HasValue)
            {
                query = query.Where(p => p.P_SucChua == sucChua.Value);
            }
            if (trangThai.HasValue)
            {
                query = query.Where(p => p.P_TrangThai == trangThai.Value);
            }
            var phongList = query.OrderBy(p => p.P_ID).ToList();

            var tbList = _context.ThongBaos
                .OrderByDescending(x => x.TB_ThoiGian)
                .Take(5)
                .ToList();

            ViewBag.ThongBao = tbList;

            ViewBag.Keyword = keyword;
            ViewBag.LoaiPhong = loaiPhong;
            ViewBag.SucChua = sucChua;
            ViewBag.TrangThai = trangThai;

            return View(phongList);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            var p = _context.Phongs.Find(id);
            if (p == null)
                return NotFound();
            return View(p);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var delPhong = _context.Phongs.Find(id);
            if (delPhong == null)
                return NotFound();
            _context.Phongs.Remove(delPhong);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Create()
        {
            var phongList = (from p in _context.Phongs
                             select new SelectListItem()
                             {
                                 Text = p.P_ID.ToString(),
                                 Value = p.P_ID.ToString()
                             }).ToList();
            phongList.Insert(0, new SelectListItem()
            {
                Text = "--- select ---",
                Value = "0"
            });
            ViewBag.PhongList = phongList;
            var images = Directory.GetFiles("wwwroot/img").Select(Path.GetFileName).ToList();

            ViewBag.Images = images;
            return View();
        }
        [HttpPost]
        public IActionResult Create(tblPhong p)
        {
            if (!ModelState.IsValid)
            {
                var images = Directory.GetFiles("wwwroot/img")
                    .Select(Path.GetFileName)
                    .ToList();

                ViewBag.Images = images;
                return View(p);
            }
            bool isExist = _context.Phongs.Any(x => x.P_TenPhong == p.P_TenPhong);
            if (isExist)
            {
                ModelState.AddModelError("P_TenPhong", "Tên phòng đã tồn tại!");
                var images = Directory.GetFiles("wwwroot/img").Select(Path.GetFileName).ToList();
                ViewBag.Images = images;
                return View(p);
            }
            _context.Phongs.Add(p);
            _context.ThongBaos.Add(new ThongBao
            {
                TB_NoiDung = "Đã thêm phòng mới: " + p.P_TenPhong,
                TB_ThoiGian = DateTime.Now,
                TB_TrangThai = false
            });
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var p = _context.Phongs.Find(id);
            if (p == null)
                return NotFound();

            var images = Directory.GetFiles("wwwroot/img")
                .Select(Path.GetFileName)
                .ToList();

            ViewBag.Images = images;

            return View(p);
        }
        [HttpPost]
        public IActionResult Edit(tblPhong p)
        {
            if (ModelState.IsValid)
            {
                var existingPhong = _context.Phongs
                    .AsNoTracking()
                    .FirstOrDefault(x => x.P_ID == p.P_ID);

                if (existingPhong == null)
                    return NotFound();

                if (string.IsNullOrEmpty(p.P_HinhAnh))
                {
                    p.P_HinhAnh = existingPhong.P_HinhAnh;
                }

                _context.Phongs.Update(p);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            var images = Directory.GetFiles("wwwroot/img")
                .Select(Path.GetFileName)
                .ToList();

            ViewBag.Images = images;

            return View(p);
        }
    }
}