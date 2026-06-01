using DoAn.Data;
using DoAn.Models;
using DoAn.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TaiKhoanController : Controller
    {
        private readonly AppDbContext _context;

        public TaiKhoanController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DangNhap(string email, string password)
        {
            var kh = _context.KhachHangs.FirstOrDefault(x => x.KH_Email == email &&
                    x.KH_MatKhau == password && x.KH_TrangThai == true);

            if (kh == null)
            {
                ViewBag.Error = "Email hoặc mật khẩu không đúng";
                return View();
            }

            Functions._KH_ID = kh.KH_ID;
            Functions._TenKhach = kh.KH_TenKhach;
            Functions._Email = kh.KH_Email;
            Functions._VaiTro = kh.KH_VaiTro ?? "";

            if (Functions._VaiTro == "Admin")
                return RedirectToAction("Index", "Home", new { area = "Admin" });

            return RedirectToAction("Index", "Home");
        }

        public IActionResult DangXuat()
        {
            Functions._KH_ID = 0;
            Functions._TenKhach = "";
            Functions._Email = "";
            Functions._VaiTro = "";

            return RedirectToAction("DangNhap");
        }
    }
}