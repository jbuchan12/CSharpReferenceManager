using DotNet.Cli.CommandLineInterfaces;

namespace DotNet.Cli.VisualStudio;

public class Project : IProject
{
    private readonly IProjectsCommandLineInterface? _projectsCommandLineInterface;
    private List<Project> _projects = new();
    private List<NugetPackage> _packages = new();
    private readonly List<ProjectChangeLog> _changes = new();

    public Project(string name, string directory, IProjectsCommandLineInterface projectsCommandLineInterface)
    {
        Id = Guid.NewGuid();
        _projectsCommandLineInterface = projectsCommandLineInterface;
        Directory = System.IO.Directory.GetParent($"{directory}/{name}")?.FullName ?? string.Empty;
        Name = Path.GetFileName(name);
    }

    public Project(string fileName, IProjectsCommandLineInterface projectsCommandLineInterface)
    {
        Id = Guid.NewGuid();
        _projectsCommandLineInterface = projectsCommandLineInterface;
        Directory = System.IO.Directory.GetParent(fileName)?.FullName ?? string.Empty;
        Name = Path.GetFileName(fileName);
    }

    public Project(string name, string directory)
    {
        Directory = directory;
        Name = name;
    }

    public Guid Id { get; set; }
    public string Name { get; }
    public string Directory { get; }
    public NugetPackage? LinkedNugetPackage { get; set; }
    public EventHandler<EventArgs>? ProjectDataChanged { get; set; }

    public Task<List<Project>> Projects
        => GetProjects();

    public async Task AddProject(Project project, ISolution solution)
    {
        await GetProjectsCommandLineInterface()
            .Projects(this)
            .Add(project, solution.Directory);

        _changes.Add(new ProjectChangeLog(ProjectChangedObject.Project, 1));
    }

    public async Task RemoveProject(Project project, ISolution solution)
    {
        await GetProjectsCommandLineInterface()
            .Projects(this)
            .Remove(project, solution.Directory);

        _changes.Add(new ProjectChangeLog(ProjectChangedObject.Project, -1));
    }

    public Task<List<NugetPackage>> Packages
        => GetPackages();

    public async Task AddPackage(NugetPackage project)
    {
        await GetProjectsCommandLineInterface().Packages(this).Add(project);

        _changes.Add(new ProjectChangeLog(ProjectChangedObject.Package, 1));
    }

    public async Task RemovePackage(NugetPackage project)
    {
        await GetProjectsCommandLineInterface().Packages(this).Remove(project);

        _changes.Add(new ProjectChangeLog(ProjectChangedObject.Package, -1));
    }

    public async Task RefreshData()
    {
        int projectsCount = _projects.Count;
        int packagesCount = _packages.Count;

        await RefreshProjectData();
        await RefreshPackageData();
        await CheckDataIntegrity(projectsCount, packagesCount);

        _changes.Clear();

        ProjectDataChanged?.Invoke(this, EventArgs.Empty);
    }

    private async Task RefreshProjectData()
    {
        _projects.Clear();
        await GetProjects();
    }

    private async Task RefreshPackageData()
    {
        _packages.Clear();
        await GetPackages();
    }

    private async Task CheckDataIntegrity(int oldProjectsCount, int oldPackagesCount)
    {
        if (!_changes.Any())
            return;

        bool projectWrong = IsProjectCountWrong(oldProjectsCount);
        bool packageWrong = IsPackageCountWrong(oldPackagesCount);

        while (projectWrong || packageWrong)
        {
            if (projectWrong)
            {
                await RefreshProjectData();
                projectWrong = IsProjectCountWrong(oldProjectsCount);
            }

            if (!packageWrong) continue;

            await RefreshPackageData();
            packageWrong = IsPackageCountWrong(oldPackagesCount);

        }
    }

    private bool IsProjectCountWrong(int oldProjectsCount)
    {
        int projectChangeCount = _changes
            .Where(x => x.ChangedObject == ProjectChangedObject.Project)
            .Sum(x => x.ChangeCount);

        return _projects.Count != oldProjectsCount + projectChangeCount;
    }

    private bool IsPackageCountWrong(int oldPackagesCount)
    {
        int packageChangeCount = _changes
            .Where(x => x.ChangedObject == ProjectChangedObject.Package)
            .Sum(x => x.ChangeCount);

        return _packages.Count != oldPackagesCount + packageChangeCount;
    }

    private async Task<List<Project>> GetProjects()
    {
        if (_projects.Any())
            return _projects;

        _projects = await GetProjectsCommandLineInterface()
            .Projects(this)
            .Get();

        return _projects;
    }

    private async Task<List<NugetPackage>> GetPackages()
    {
        if (_packages.Any())
            return _packages;

        _packages = await GetProjectsCommandLineInterface()
            .Packages(this)
            .Get();

        return _packages;
    }

    private IProjectsCommandLineInterface GetProjectsCommandLineInterface()
    {
        if (_projectsCommandLineInterface is null)
            throw new InvalidOperationException("No projects command line interface");

        return _projectsCommandLineInterface;
    }
}

public interface IProject : IVisualStudioObject
{
    Task<List<NugetPackage>> Packages { get; }
    EventHandler<EventArgs>? ProjectDataChanged { get; set; }
    Task AddPackage(NugetPackage project);
    Task RemovePackage(NugetPackage project);
    Task AddProject(Project project, ISolution solution);
}