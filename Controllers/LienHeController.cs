using Microsoft.AspNetCore.Mvc;
using DoAn.Models;
using DoAn.Data;
using System;

public class LienHeController : Controller
{
    private readonly AppDbContext _context;
    public LienHeController(AppDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Index(string Ten, string Email, string NoiDung)
    {
        _context.LienHes.Add(new LienHe
        {
            LH_TenKhach = Ten,
            LH_Email = Email,
            LH_NoiDung = NoiDung,
            LH_ThoiGian = DateTime.Now,
            LH_TrangThai = false
        });
        _context.SaveChanges();
        TempData["LienHeSuccess"] = "Gửi liên hệ thành công!";
        return RedirectToAction("Index");
    }
}