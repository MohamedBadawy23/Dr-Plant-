
using Core;
using Core.Enteties;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using System.Reflection;

namespace ASP.Authentication.Data;

public class ApplicationDbContext (DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<AppUser>(options)

{
    protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);
    builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
}

public DbSet<PlantImage> PlanetImages { get; set; }
    public DbSet<DiagnosisResult> DiagnosisResults { get; set; }
    public DbSet<ReportProblem> ReportProblems { get; set; }
    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    //}

}


