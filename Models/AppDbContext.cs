using DoAn.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<tblMenu> Menus { get; set; }
    public DbSet<tblRoom> Rooms { get; set; }
    public DbSet<tblReview> Reviews { get; set; }
}