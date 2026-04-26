using Microsoft.AspNetCore.Mvc;
using DoAn.Models;
using DoAn.Data;
using System.Linq;

public class MenuViewComponent : ViewComponent
{
    private readonly AppDbContext _context;

    public MenuViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public IViewComponentResult Invoke()
    {
        var menu = _context.Menus
            .Where(m => m.IsActive)
            .OrderBy(m => m.MenuOrder)
            .ToList();

        return View(menu);
    }
}