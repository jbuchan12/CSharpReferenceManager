using CsSolutionManger.Console.Interfaces;
using System.IO;
using System.Xml.Linq;

namespace CsSolutionManger.Console.Models;

public class Project : IProject
{
    private readonly IProjectsCliApi _projectsCliApi;

    public Project(string name, string directory, IProjectsCliApi projectsCliApi)
    {
        Id = Guid.NewGuid();
        _projectsCliApi = projectsCliApi;
        Directory = System.IO.Directory.GetParent($"{directory}/{name}")?.FullName ?? string.Empty;
        Name = Path.GetFileName(name);
    }

    public Project(string fileName, IProjectsCliApi projectsCliApi)
    {
        Id = Guid.NewGuid();
        _projectsCliApi = projectsCliApi;
        Directory = System.IO.Directory.GetParent(fileName)?.FullName ?? string.Empty;
        Name = Path.GetFileName(fileName);
    }

    public Guid Id { get; set; }
    public string Name { get; }
    public string Directory { get; }

    public Task<List<Project>> Projects
        => _projectsCliApi.Projects(this).Get();

    public async Task AddProject(Project project, ISolution solution)
        => await _projectsCliApi.Projects(this).Add(project, solution.Directory);

    public async Task RemoveProject(Project project)
        => await _projectsCliApi.Projects(this).Remove(project);

    public Task<List<NugetPackage>> Packages
        => _projectsCliApi.Packages(this).Get();

    public Task AddPackage(NugetPackage project)
        => _projectsCliApi.Packages(this).Add(project);

    public Task RemovePackage(NugetPackage project)
        => _projectsCliApi.Packages(this).Remove(project);
}