using DotNet.Cli.CommandLineInterfaces;

namespace DotNet.Cli.VisualStudio;

public class Project : IProject
{
    private readonly IProjectsCommandLineInterface? _projectsCommandLineInterface;
    private List<Project> _projects = new();
    private List<NugetPackage> _packages = new();
    private List<ProjectChangeLog> _changes = new();

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

        _projects.Clear();
        _= await GetProjects();
    }

    public async Task RemoveProject(Project project, ISolution solution)
    {
        await GetProjectsCommandLineInterface()
            .Projects(this)
            .Remove(project, solution.Directory);

        _changes.Add(new ProjectChangeLog(ProjectChangedObject.Project, -1));

        _projects.Clear();
        await GetProjects();
    }

    public Task<List<NugetPackage>> Packages
        => GetPackages();

    public async Task AddPackage(NugetPackage project)
    {
        await GetProjectsCommandLineInterface().Packages(this).Add(project);

        _changes.Add(new ProjectChangeLog(ProjectChangedObject.Package, 1));
        _packages.Clear();

        await GetPackages();
    }

    public async Task RemovePackage(NugetPackage project)
    {
        await GetProjectsCommandLineInterface().Packages(this).Remove(project);

        _changes.Add(new ProjectChangeLog(ProjectChangedObject.Package, -1));
        _packages.Clear();

        await GetPackages();
    }

    public async Task RefreshData()
    {
         _projects.Clear();
        _packages.Clear();

        await GetProjects();
        await GetPackages();

        ProjectDataChanged?.Invoke(this, EventArgs.Empty);
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

public class ProjectChangeLog
{
    public ProjectChangeLog(ProjectChangedObject changedObject, int changeCount)
    {
        ChangedObject = changedObject;
        ChangeCount = changeCount;
    }

    public ProjectChangedObject ChangedObject { get; set; }
    public int ChangeCount { get; set; }
}

public enum ProjectChangedObject
{
    None,
    Project,
    Package
}

public interface IProject : IVisualStudioObject
{
    Task<List<NugetPackage>> Packages { get; }
    EventHandler<EventArgs>? ProjectDataChanged { get; set; }
    Task AddPackage(NugetPackage project);
    Task RemovePackage(NugetPackage project);
    Task AddProject(Project project, ISolution solution);
}