using E_Clubs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_Clubs.Data;

public class AppDbContext : DbContext
{

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });

        builder.Entity<Role>().HasData(
            new Role { Id = Guid.NewGuid(), Name = "Admin", Description = "Admin Description" },
            new Role { Id = Guid.NewGuid(), Name = "Director", Description = "Director Description" },
            new Role { Id = Guid.NewGuid(), Name = "Professor", Description = "Professor Description" },
            new Role { Id = Guid.NewGuid(), Name = "Student", Description = "Student Description" }
        );
    }
}