using CsSolutionManager.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CsSolutionManager.DataLayer;

public sealed class CsSolutionManagerContext : DbContext, ICsSolutionManagerContext
{
    public DbSet<ApplicationHistory> ApplicationHistory { get; set; } = null!;
    public DbSet<Solution> Solutions { get; set; } = null!;
    public DbSet<NugetPackage> NugetPackages { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;

    public CsSolutionManagerContext() : base(SqliteContextOptions)
    {
        Database.Migrate();
    }

    private static DbContextOptions SqliteContextOptions
        => new DbContextOptionsBuilder<CsSolutionManagerContext>()
            .UseSqlite($"Data Source={GetDbPath()}")
            .Options;

    private static string GetDbPath()
    {
        string appFolder = AppDomain.CurrentDomain.BaseDirectory;

        if (!Directory.Exists(appFolder))
            Directory.CreateDirectory(appFolder);

        return Path.Join(appFolder, "voyager.db");
    }

    public DbSet<TEntity> Table<TEntity>() where TEntity : class, IEntity
        => Set<TEntity>();
}

public interface ICsSolutionManagerContext
{
    DbSet<ApplicationHistory> ApplicationHistory { get; set; }
    DbSet<Solution> Solutions { get; set; }
    DbSet<NugetPackage> NugetPackages { get; set; }
    DbSet<Project> Projects { get; set; }
    DatabaseFacade Database { get; }
    DbSet<TEntity> Table<TEntity>() where TEntity : class, IEntity;
    int SaveChanges();
}