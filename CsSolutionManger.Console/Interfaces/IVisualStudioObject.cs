using CsSolutionManger.Console.Models;

namespace CsSolutionManger.Console.Interfaces;

public interface IVisualStudioObject
{
    Guid Id { get; set; }
    string Name { get; }
    string Directory { get; }
    Task RemoveProject(Project project);
}