using CsSolutionManger.Console.Models;

namespace CsSolutionManger.Console.Interfaces;

public interface IProject : IVisualStudioObject
{
    Task<List<NugetPackage>> Packages { get; }
    Task AddPackage(NugetPackage project);
    Task RemovePackage(NugetPackage project);
    Task AddProject(Project project, ISolution solution);
}