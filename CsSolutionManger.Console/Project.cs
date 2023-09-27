using CsSolutionManger.Console.DotNetCli;

namespace CsSolutionManger.Console;

public class Project
{
    private readonly ProjectsCliApi _projectsCliApi;

    public Project(string name, string directory, ProjectsCliApi projectsCliApi)
    {
        _projectsCliApi = projectsCliApi;
        Directory = System.IO.Directory.GetParent($"{directory}/{name}")?.FullName ?? string.Empty;
        Name = Path.GetFileName(name);
    }

    public string Name { get; }
    public string Directory { get; }

    public List<NugetPackage> Packages 
        => _projectsCliApi.Packages(this).Get();

    public void AddProject(NugetPackage project)
        => _projectsCliApi.Packages(this).Add(project);

    public void RemoveProject(NugetPackage project)
        => _projectsCliApi.Packages(this).Remove(project);

}

public class NugetPackage
{
    public NugetPackage(string name)
    {
        Name = name;
    }

    public readonly string Name;
}