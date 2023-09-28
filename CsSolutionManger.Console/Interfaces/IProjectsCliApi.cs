using CsSolutionManger.Console.DotNetCli;

namespace CsSolutionManger.Console.Interfaces;

public interface IProjectsCliApi
{
    PackagesCliApi Packages(Project project);
    ProjectsCliApi<IProject> Projects(Project project);
    List<Project> Get();
    void Add(Project project);
    void Remove(Project project);
    string Command { get; }
}