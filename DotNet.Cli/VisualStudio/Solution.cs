using DotNet.Cli.CommandLineInterfaces;

namespace DotNet.Cli.VisualStudio;

public class Solution : ISolution
{
    private readonly SolutionCommandLineInterface _solutionCommandLineInterface;

    public Solution(string solutionFullPath, DotNetCommandLineInterface dotNetCommandLineInterface)
    {
        if (!File.Exists(solutionFullPath))
            throw new InvalidOperationException("Solution file not found at path");

        FullPath = solutionFullPath;
        Id = Guid.NewGuid();

        _solutionCommandLineInterface = dotNetCommandLineInterface.Solution(this);
    }

    public Guid Id { get; set; }
    public string FullPath { get; }
    public string Directory => System.IO.Directory.GetParent(FullPath)?.FullName ?? string.Empty;
    public string Name => Path.GetFileName(FullPath);

    public Task<List<Project>> Projects
        => _solutionCommandLineInterface.Projects.Get();

    public Task AddProject(Project project)
        => _solutionCommandLineInterface.Projects.Add(project, Directory);

    public Task RemoveProject(Project project)
        => _solutionCommandLineInterface.Projects.Remove(project);
}

public interface ISolution : IVisualStudioObject
{
    string FullPath { get; }
    Task<List<Project>> Projects { get; }
    Task AddProject(Project project);
}