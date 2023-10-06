using CsSolutionManger.Console.Models;

namespace CsSolutionManger.Console.Interfaces;

public interface ISolution : IVisualStudioObject
{
    string FullPath { get; }
    Task<List<Project>> Projects { get; }
    Task AddProject(Project project);
}