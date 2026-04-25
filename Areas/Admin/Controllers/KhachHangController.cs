using DoAn.Models;
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
        public IActionResult Index()
        {
            var khachHangList = _context.KhachHangs.OrderBy(k => k.KH_ID).ToList();
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
    }
}