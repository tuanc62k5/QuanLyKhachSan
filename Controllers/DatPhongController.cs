using Microsoft.AspNetCore.Mvc;
using DoAn.Models;
using DoAn.Data;
using System;
using System.Linq;

public class DatPhongController : Controller
{
    private readonly AppDbContext _context;

    public DatPhongController(AppDbContext context)
    {
        _context = context;
    }

    // GET
    public IActionResult Index(int P_ID)
    {
        var phong = _context.Phongs.FirstOrDefault(p => p.P_ID == P_ID);
        ViewBag.Phong = phong;

        return View();
    }

    // POST
    [HttpPost]
    public IActionResult Index(tblDatPhong model)
    {
        var userId = HttpContext.Session.GetInt32("UserID");

        if (userId == null)
        {
            return RedirectToAction("DangNhap", "TaiKhoan");
        }
        var phong = _context.Phongs.FirstOrDefault(p => p.P_ID == model.P_ID);

        if (phong == null)
            return NotFound();

        var soGio = (model.DP_NgayTra - model.DP_NgayNhan).TotalHours;

        if (soGio <= 0)
        {
            ModelState.AddModelError("", "Giờ trả phải lớn hơn giờ nhận");
            ViewBag.Phong = phong;
            return View(model);
        }

        soGio = Math.Ceiling(soGio);

        decimal tongTien = (decimal)soGio * phong.P_GiaPhong;

        model.DP_TongTien = tongTien;
        model.DP_NgayTao = DateTime.Now;
        model.KH_ID = HttpContext.Session.GetInt32("UserID");

        _context.DatPhongs.Add(model);
        _context.SaveChanges();

        return RedirectToAction("ThanhCong");
    }

    public IActionResult ThanhCong()
    {
        return View();
    }
}