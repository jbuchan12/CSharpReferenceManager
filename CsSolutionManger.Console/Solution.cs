using CsSolutionManger.Console.DotNetCli;

namespace CsSolutionManger.Console;

public class Solution : ISolution
{
    private readonly SolutionCliApi _solutionCliApi;

    public Solution(string solutionFullPath, DotNetCli.DotNetCli dotNetCli)
    {
        if(! File.Exists(solutionFullPath))
            throw new InvalidOperationException("Solution file not found at path");

        FullPath = solutionFullPath;

        _solutionCliApi = dotNetCli.Solution(this);
    }

    public string FullPath { get;}
    public string Directory => System.IO.Directory.GetParent(FullPath)?.FullName ?? string.Empty;
    public string Name => Path.GetFileName(FullPath);

    public List<Project> Projects 
        => _solutionCliApi.Projects.Get();

    public void AddProject(Project project) 
        => _solutionCliApi.Projects.Add(project);

    public void RemoveProject(Project project) 
        => _solutionCliApi.Projects.Remove(project);
}

public interface ISolution
{
    string FullPath { get; }
    List<Project> Projects { get; }
    string Directory { get; }
    string Name { get; }

    void AddProject(Project project);
    void RemoveProject(Project project);
}