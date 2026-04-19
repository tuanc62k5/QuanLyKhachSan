using Microsoft.AspNetCore.Mvc;
using DoAn.Models;
using System.Linq;

public class LienHeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Index(string Ten, string Email, string NoiDung)
    {
        // tạm thời chỉ hiển thị thông báo
        TempData["LienHeSuccess"] = "Gửi liên hệ thành công! Chúng tôi sẽ phản hồi sớm.";

        return RedirectToAction("Index");
    }
}