namespace CsSolutionManger.Console.Interfaces;

public interface IVisualStudioObject
{
    Guid Id { get; set; }
    string Name { get; }
    string Directory { get; }
    void AddProject(Project project);
    void RemoveProject(Project project);
}