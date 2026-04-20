using DoAn.Areas.Admin.Models;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<tblMenu> Menus { get; set; }
        public DbSet<AdminMenu> AdminMenus { get; set; }
    }
}