using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DoAn.Models;

namespace aznews.Controllers;

public class PhongController : Controller
{
    private readonly AppDbContext _context;

    public PhongController(AppDbContext context)
    {
        _context = context;
    }

    [Route("Rooms")]
    public IActionResult DanhSach(DateTime? checkIn, DateTime? checkOut, int adults = 1, int children = 0)
    {
        int totalPeople = adults + children;

        var rooms = _context.Rooms
            .Where(r => r.IsActive && r.Capacity >= totalPeople)
            .OrderBy(r => r.Price)
            .ToList();

        return View(rooms);
    }
}