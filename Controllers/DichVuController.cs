using Microsoft.AspNetCore.Mvc;
using DoAn.Models;
using DoAn.Data;
using System.Linq;

public class DichVuController : Controller
{
    private readonly AppDbContext _context;

    public DichVuController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(int? dpId)
    {
        ViewBag.DP_ID = dpId;

        var list = _context.DichVus.ToList();

        return View(list);
    }
}