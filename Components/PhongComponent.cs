using Microsoft.AspNetCore.Mvc;
using DoAn.Models;
using DoAn.Data;

namespace DoAn.Components;

[ViewComponent(Name = "Phong")]
public class PhongViewComponent : ViewComponent
{
    private readonly AppDbContext _context;

    public PhongViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public IViewComponentResult Invoke()
    {
        var phongs = _context.Phongs.Where(p => p.P_TrangThai).OrderBy(p => p.P_ID).Take(3).ToList();
        return View(phongs);
    }
}