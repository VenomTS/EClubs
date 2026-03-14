using E_Clubs.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Clubs.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    public DbSet<Club> Clubs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });

        builder.Entity<Role>().HasData(
            new Role { Id = Guid.NewGuid(), Name = "Admin" },
            new Role { Id = Guid.NewGuid(), Name = "Director" },
            new Role { Id = Guid.NewGuid(), Name = "Professor" },
            new Role { Id = Guid.NewGuid(), Name = "Student" }
        );
        
        builder.Entity<Club>().Property(club => club.CreatedAt).ValueGeneratedOnAdd().HasDefaultValueSql("NOW()");
    }
}