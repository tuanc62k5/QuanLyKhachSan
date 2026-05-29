using Microsoft.AspNetCore.Mvc;
using DoAn.Data;
using System.Linq;

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
            var admin = _context.KhachHangs.FirstOrDefault(x =>
                x.KH_Email == email &&
                x.KH_MatKhau == password &&
                x.KH_VaiTro != null &&
                x.KH_VaiTro.Trim().ToLower() == "admin"
            );

            if (admin == null)
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
                return View();
            }

            HttpContext.Session.SetString("Admin", admin.KH_TenKhach);
            HttpContext.Session.SetInt32("AdminID", admin.KH_ID);

            return Redirect("/Admin/Home");
        }

        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();

            return Redirect("/Admin/TaiKhoan/DangNhap");
        }
    }
}