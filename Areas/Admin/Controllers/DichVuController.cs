using DoAn.Models;
using DoAn.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DichVuController : Controller
    {
        private readonly AppDbContext _context;

        public DichVuController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string keyword, bool? trangThai)
        {
            var query = _context.DichVus.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(dv =>
                    dv.DV_TenDichVu.Contains(keyword));
            }

            if (trangThai.HasValue)
            {
                query = query.Where(dv => dv.DV_TrangThai == trangThai.Value);
            }
            var dichvuList = query.OrderBy(dv => dv.DV_ID).ToList();

            var tbList = _context.ThongBaos.OrderByDescending(x => x.TB_ThoiGian).Take(5).ToList();

            ViewBag.ThongBao = tbList;

            ViewBag.Keyword = keyword;
            ViewBag.TrangThai = trangThai;

            return View(dichvuList);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var dv = _context.DichVus.Find(id);

            if (dv == null)
                return NotFound();

            var tbList = _context.ThongBaos.OrderByDescending(x => x.TB_ThoiGian).Take(5).ToList();

            ViewBag.ThongBao = tbList;

            return View(dv);
        }

        [HttpPost]
        public IActionResult Delete(int id, string? confirm)
        {
            var delDichVu = _context.DichVus.Find(id);

            if (delDichVu == null)
                return NotFound();

            _context.DichVus.Remove(delDichVu);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Create()
        {
            var tbList = _context.ThongBaos.OrderByDescending(x => x.TB_ThoiGian).Take(5).ToList();

            ViewBag.ThongBao = tbList;

            var imgFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");

            if (Directory.Exists(imgFolder))
            {
                ViewBag.Images = Directory.GetFiles(imgFolder).Select(x => Path.GetFileName(x)!)
                    .Where(x =>
                        x.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                        x.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                        x.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                        x.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            else
            {
                ViewBag.Images = new List<string>();
            }

            return View();
        }

        [HttpPost]
        public IActionResult Create(tblDichVu dv)
        {
            if (string.IsNullOrWhiteSpace(dv.DV_TenDichVu))
                ModelState.AddModelError("DV_TenDichVu", "Vui lòng nhập tên dịch vụ!");

            if (dv.DV_GiaTien < 0)
                ModelState.AddModelError("DV_GiaTien", "Giá tiền không hợp lệ!");

            if (string.IsNullOrWhiteSpace(dv.DV_HinhAnh))
                ModelState.AddModelError("DV_HinhAnh", "Vui lòng chọn ảnh!");

            if (ModelState.IsValid)
            {
                _context.DichVus.Add(dv);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            var tbList = _context.ThongBaos.OrderByDescending(x => x.TB_ThoiGian).Take(5).ToList();

            ViewBag.ThongBao = tbList;

            var imgFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");

            if (Directory.Exists(imgFolder))
            {
                ViewBag.Images = Directory.GetFiles(imgFolder).Select(x => Path.GetFileName(x)!)
                    .Where(x =>
                        x.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                        x.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                        x.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                        x.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            else
            {
                ViewBag.Images = new List<string>();
            }

            return View(dv);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var dv = _context.DichVus.Find(id);

            if (dv == null)
                return NotFound();

            var tbList = _context.ThongBaos.OrderByDescending(x => x.TB_ThoiGian).Take(5).ToList();

            ViewBag.ThongBao = tbList;

            var imgFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");

            if (Directory.Exists(imgFolder))
            {
                ViewBag.Images = Directory.GetFiles(imgFolder).Select(x => Path.GetFileName(x)!)
                    .Where(x =>
                        x.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                        x.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                        x.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                        x.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            else
            {
                ViewBag.Images = new List<string>();
            }

            return View(dv);
        }

        [HttpPost]
        public IActionResult Edit(tblDichVu dv)
        {
            if (string.IsNullOrWhiteSpace(dv.DV_TenDichVu))
                ModelState.AddModelError("DV_TenDichVu", "Vui lòng nhập tên dịch vụ!");

            if (dv.DV_GiaTien < 0)
                ModelState.AddModelError("DV_GiaTien", "Giá tiền không hợp lệ!");

            if (string.IsNullOrWhiteSpace(dv.DV_HinhAnh))
                ModelState.AddModelError("DV_HinhAnh", "Vui lòng chọn ảnh!");

            if (ModelState.IsValid)
            {
                _context.DichVus.Update(dv);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            var tbList = _context.ThongBaos.OrderByDescending(x => x.TB_ThoiGian).Take(5).ToList();

            ViewBag.ThongBao = tbList;

            var imgFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");

            if (Directory.Exists(imgFolder))
            {
                ViewBag.Images = Directory.GetFiles(imgFolder).Select(x => Path.GetFileName(x)!)
                    .Where(x =>
                        x.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                        x.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                        x.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                        x.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            else
            {
                ViewBag.Images = new List<string>();
            }

            return View(dv);
        }

        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var dv = _context.DichVus.FirstOrDefault(x => x.DV_ID == id);

            if (dv == null)
                return NotFound();

            return View(dv);
        }
    }
}