using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_Clubs.Data;

public class AppDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    private static readonly List<IdentityRole> IdentityRoles =
    [
        new() { Name = AppRoles.Admin, NormalizedName = AppRoles.Admin.ToUpper() },
        new() { Name = AppRoles.Director, NormalizedName = AppRoles.Director.ToUpper() },
        new() { Name = AppRoles.Teacher, NormalizedName = AppRoles.Teacher.ToUpper() },
        new() { Name = AppRoles.Student, NormalizedName = AppRoles.Student.ToUpper() }
    ];

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<IdentityRole>().HasData(IdentityRoles);
    }
}