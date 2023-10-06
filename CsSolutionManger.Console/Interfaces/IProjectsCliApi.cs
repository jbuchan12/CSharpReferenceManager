using CsSolutionManger.Console.DotNetCli;
using CsSolutionManger.Console.Models;

namespace CsSolutionManger.Console.Interfaces;

public interface IProjectsCliApi
{
    PackagesCliApi Packages(Project project);
    ProjectsCliApi<IProject> Projects(Project project);
    Task<List<Project>> Get();
    Task Add(Project project, string solutionDirectory);
    Task Remove(Project project);
    string Command { get; }
}