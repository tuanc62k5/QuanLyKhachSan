using DoAn.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<tblMenu> Menus { get; set; }
    public DbSet<tblPhong> Phongs { get; set; }
    public DbSet<tblGioiThieu> GioiThieus { get; set; }
}