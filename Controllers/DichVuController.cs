using Microsoft.AspNetCore.Mvc;
using DoAn.Models;
using System.Linq;

public class DichVuController : Controller
{
    private readonly AppDbContext _context;

    public DichVuController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var dichvus = _context.DichVus
            .Where(d => d.DV_TrangThai)
            .ToList();

        return View(dichvus);
    }
}