using CsSolutionManager.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace CsSolutionManager.DataLayer.Repositories;

public class NugetPackageRepository : GenericRepository<NugetPackage>, INugetPackageRepository
{
    public NugetPackageRepository(ICsSolutionManagerContext dbContext) : base(dbContext)
    {
    }

    public override NugetPackage? GetByName(string name) => base.Get()
        .Include(x => x.Project)
        .SingleOrDefault(x => x.Name == name);
}

public interface INugetPackageRepository
{
    IQueryable<NugetPackage> Get();
    NugetPackage? GetById(Guid id);
    NugetPackage? GetByName(string name);
    int Add(NugetPackage entity);
    int Remove(NugetPackage entity);
}