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
            return View(dv);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var delDichVu = _context.DichVus.Find(id);
            if (delDichVu == null)
                return NotFound();
            _context.DichVus.Remove(delDichVu);
            _context.SaveChanges();
            return RedirectToAction("Index");
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