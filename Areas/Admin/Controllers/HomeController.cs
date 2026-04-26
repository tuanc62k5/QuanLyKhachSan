using Microsoft.AspNetCore.Mvc;
using DoAn.Models;
using DoAn.Data;

namespace DoAn.Areas.Admin.Controllers;

[Area("Admin")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}