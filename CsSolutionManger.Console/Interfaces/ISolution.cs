namespace CsSolutionManger.Console.Interfaces;

public interface ISolution : IVisualStudioObject
{
    string FullPath { get; }
    List<Project> Projects { get; }
}