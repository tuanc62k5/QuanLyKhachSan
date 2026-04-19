using Microsoft.AspNetCore.Mvc;
using DoAn.Models;
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
        var phong = _context.Phongs.FirstOrDefault(p => p.P_ID == model.P_ID);

        if (phong != null)
        {
            int soNgay = (model.DP_NgayTra - model.DP_NgayNhan).Days;
            if (soNgay <= 0) soNgay = 1;

            model.DP_TongTien = soNgay * phong.P_GiaPhong;
            model.DP_TienCoc = model.DP_TongTien * 0.3m;
            model.DP_NgayTao = DateTime.Now;

            _context.DatPhongs.Add(model);
            _context.SaveChanges();

            return RedirectToAction("ThanhCong");
        }

        return View(model);
    }

    public IActionResult ThanhCong()
    {
        return View();
    }
}