using DoAn.Models;
using Microsoft.AspNetCore.Mvc;


namespace DoAn.Areas.Admin.Components
{
    [ViewComponent(Name = "AdminMenu")]
    public class AdminMenuComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        public AdminMenuComponent(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var mnList = (from mn in _context.AdminMenus
                          where (mn.IsActive == true)
                          select mn).ToList();
            return await Task.FromResult((IViewComponentResult)View("Default", mnList));
        }
    }
}