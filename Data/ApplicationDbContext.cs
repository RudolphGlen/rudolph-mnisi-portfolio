using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BeginnerMvc.Models;

namespace BeginnerMvc.Data;

// Provides Entity Framework Core access to application data and ASP.NET Core Identity.
// IdentityDbContext includes the tables needed for users, roles, claims, and logins.
public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<Skill> Skills { get; set; } = null!;
    public DbSet<Experience> Experiences { get; set; } = null!;
    public DbSet<Certification> Certifications { get; set; } = null!;

    // Receive the database provider and options configured by dependency injection.
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}
