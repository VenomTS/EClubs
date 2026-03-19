using E_Clubs.Clubs;
using E_Clubs.Users;
using E_Clubs.WorkPlans;
using Microsoft.EntityFrameworkCore;

namespace E_Clubs.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Club> Clubs { get; set; }
    public DbSet<WorkPlan> WorkPlans { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<Club>().Property(club => club.CreatedAt).ValueGeneratedOnAdd().HasDefaultValueSql("NOW()");
    }
}