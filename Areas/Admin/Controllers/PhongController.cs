using DoAn.Models;
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
        public IActionResult Index()
        {
            var phongList = _context.Phongs.OrderBy(p => p.P_ID).ToList();
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
            return View();
        }
        [HttpPost]
        public IActionResult Create(tblPhong p, IFormFile ImageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(p);
            }
            bool isExist = _context.Phongs.Any(x => x.P_TenPhong == p.P_TenPhong);
            if (isExist)
            {
                ModelState.AddModelError("P_TenPhong", "Tên phòng đã tồn tại!");
                return View(p);
            }
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }
                p.P_HinhAnh = fileName;
            }
            _context.Phongs.Add(p);
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

            var phongList = (from ph in _context.Phongs
                             select new SelectListItem()
                             {
                                 Text = ph.P_ID.ToString(),
                                 Value = ph.P_ID.ToString()
                             }).ToList();
            phongList.Insert(0, new SelectListItem()
            {
                Text = "--- select ---",
                Value = "0"
            });
            ViewBag.PhongList = phongList;
            return View(p);
        }
        [HttpPost]
        public IActionResult Edit(tblPhong p, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                var existingPhong = _context.Phongs.AsNoTracking().FirstOrDefault(x => x.P_ID == p.P_ID);
                if (existingPhong == null)
                    return NotFound();

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        ImageFile.CopyTo(stream);
                    }
                    p.P_HinhAnh = fileName;
                }
                else
                {
                    p.P_HinhAnh = existingPhong.P_HinhAnh;
                }
                _context.Phongs.Update(p);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(p);
        }
    }
}