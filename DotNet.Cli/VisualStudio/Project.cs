using DotNet.Cli.CommandLineInterfaces;

namespace DotNet.Cli.VisualStudio;

public class Project : IProject
{
    private readonly IProjectsCommandLineInterface? _projectsCommandLineInterface;

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

    public Task<List<Project>> Projects
        => GetProjectsCommandLineInterface().Projects(this).Get();

    public async Task AddProject(Project project, ISolution solution)
        => await GetProjectsCommandLineInterface().Projects(this).Add(project, solution.Directory);

    public async Task RemoveProject(Project project)
        => await GetProjectsCommandLineInterface().Projects(this).Remove(project);

    public Task<List<NugetPackage>> Packages
        => GetProjectsCommandLineInterface().Packages(this).Get();

    public Task AddPackage(NugetPackage project)
        => GetProjectsCommandLineInterface().Packages(this).Add(project);

    public Task RemovePackage(NugetPackage project)
        => GetProjectsCommandLineInterface().Packages(this).Remove(project);

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
    Task AddPackage(NugetPackage project);
    Task RemovePackage(NugetPackage project);
    Task AddProject(Project project, ISolution solution);
}