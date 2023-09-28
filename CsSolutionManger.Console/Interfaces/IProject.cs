namespace CsSolutionManger.Console.Interfaces;

public interface IProject : IVisualStudioObject
{
    List<NugetPackage> Packages { get; }
    void AddPackage(NugetPackage project);
    void RemovePackage(NugetPackage project);
}