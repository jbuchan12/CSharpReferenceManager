using CsSolutionManger.Console.Interfaces;

namespace CsSolutionManger.Console;

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

    public Guid Id { get; set; }
    public string Name { get; }
    public string Directory { get; }

    public List<Project> Projects
        => _projectsCliApi.Projects(this).Get();

    public void AddProject(Project project)
        => _projectsCliApi.Projects(this).Add(project);

    public void RemoveProject(Project project)
        => _projectsCliApi.Projects(this).Remove(project);

    public List<NugetPackage> Packages 
        => _projectsCliApi.Packages(this).Get();

    public void AddPackage(NugetPackage project)
        => _projectsCliApi.Packages(this).Add(project);

    public void RemovePackage(NugetPackage project)
        => _projectsCliApi.Packages(this).Remove(project);

}

public class NugetPackage
{
    public NugetPackage(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
}