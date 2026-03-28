using Microsoft.AspNetCore.Mvc;
using DoAn.Models;

namespace DoAn.Components;

[ViewComponent(Name = "Room")]
public class RoomViewComponent : ViewComponent
{
    private readonly AppDbContext _context;

    public RoomViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public IViewComponentResult Invoke()
    {
        var rooms = _context.Rooms.ToList();
        return View(rooms);
    }
}