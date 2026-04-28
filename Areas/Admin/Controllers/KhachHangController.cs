using DoAn.Models;
using DoAn.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KhachHangController : Controller
    {
        private readonly AppDbContext _context;

        public KhachHangController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string keyword, string vaiTro, bool? trangThai)
        {
            var query = _context.KhachHangs.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(k =>
                    k.KH_TenKhach.Contains(keyword) || k.KH_Email.Contains(keyword)
                );
            }

            if (!string.IsNullOrEmpty(vaiTro))
            {
                query = query.Where(k => k.KH_VaiTro == vaiTro);
            }
            if (trangThai.HasValue)
            {
                query = query.Where(k => k.KH_TrangThai == trangThai.Value);
            }
            var khachHangList = query.OrderBy(k => k.KH_ID).ToList();

            var tbList = _context.ThongBaos.OrderByDescending(x => x.TB_ThoiGian).Take(5).ToList();

            ViewBag.ThongBao = tbList;

            ViewBag.Keyword = keyword;
            ViewBag.VaiTro = vaiTro;
            ViewBag.TrangThai = trangThai;

            return View(khachHangList);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            var kh = _context.KhachHangs.Find(id);
            if (kh == null)
                return NotFound();
            return View(kh);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var delKhachHang = _context.KhachHangs.Find(id);
            if (delKhachHang == null)
                return NotFound();
            _context.KhachHangs.Remove(delKhachHang);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Create()
        {
            var khachhangList = (from kh in _context.KhachHangs
                             select new SelectListItem()
                             {
                                 Text = kh.KH_TenKhach,
                                 Value = kh.KH_ID.ToString()
                             }).ToList();
            khachhangList.Insert(0, new SelectListItem()
            {
                Text = "--- select ---",
                Value = "0"
            });
            ViewBag.KhachHangList = khachhangList;
            var images = Directory.GetFiles("wwwroot/img").Select(Path.GetFileName).ToList();

            ViewBag.Images = images;
            return View();
        }
        [HttpPost]
        public IActionResult Create(tblKhachHang kh)
        {
            if (!ModelState.IsValid)
            {
                var images = Directory.GetFiles("wwwroot/img")
                    .Select(Path.GetFileName)
                    .ToList();

                ViewBag.Images = images;
                return View(kh);
            }
            bool isExist = _context.KhachHangs.Any(x => x.KH_Email == kh.KH_Email);
            if (isExist)
            {
                ModelState.AddModelError("KH_Email", "Email đã tồn tại!");
                var images = Directory.GetFiles("wwwroot/img").Select(Path.GetFileName).ToList();
                ViewBag.Images = images;
                return View(kh);
            }
            _context.KhachHangs.Add(kh);
            _context.ThongBaos.Add(new ThongBao
            {
                TB_NoiDung = "Đã thêm khách hàng mới: " + kh.KH_TenKhach,
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

            var kh = _context.KhachHangs.Find(id);
            if (kh == null)
                return NotFound();

            var images = Directory.GetFiles("wwwroot/img")
                .Select(Path.GetFileName)
                .ToList();

            ViewBag.Images = images;

            return View(kh);
        }
        [HttpPost]
        public IActionResult Edit(tblKhachHang kh)
        {
            if (ModelState.IsValid)
            {
                var existingKhachHang = _context.KhachHangs
                    .AsNoTracking()
                    .FirstOrDefault(x => x.KH_ID == kh.KH_ID);

                if (existingKhachHang == null)
                    return NotFound();

                if (string.IsNullOrEmpty(kh.KH_HinhAnh))
                {
                    kh.KH_HinhAnh = existingKhachHang.KH_HinhAnh;
                }

                _context.KhachHangs.Update(kh);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            var images = Directory.GetFiles("wwwroot/img")
                .Select(Path.GetFileName)
                .ToList();

            ViewBag.Images = images;

            return View(kh);
        }
    }
}