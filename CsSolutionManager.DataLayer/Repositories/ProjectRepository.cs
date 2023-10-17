using CsSolutionManager.DataLayer.Entities;

namespace CsSolutionManager.DataLayer.Repositories;

public class ProjectRepository : GenericRepository<Project>, IProjectRepository
{
    private readonly INugetPackageRepository _nugetPackageRepository;

    public ProjectRepository(ICsSolutionManagerContext dbContext, INugetPackageRepository nugetPackageRepository) : base(dbContext)
    {
        _nugetPackageRepository = nugetPackageRepository;
    }

    public Project? GetIncludingNugetPackage(string name)
    {
        Project? project = GetByName(name);
        if (project is null)
            return null;

        NugetPackage? linkedNugetPackage = _nugetPackageRepository.GetById(project.NugetPackageId);
        if (linkedNugetPackage is null)
            return project;

        project.NugetPackage = linkedNugetPackage;
        return project;
    }
}

public interface IProjectRepository
{
    IQueryable<Project> Get();
    Project? GetById(Guid id);
    Project? GetByName(string name);
    int Add(Project entity);
    int Remove(Project entity);
    Project? GetIncludingNugetPackage(string name);
}