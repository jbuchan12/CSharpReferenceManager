using CsSolutionManger.Console.DotNetCli;
using CsSolutionManger.Console.Interfaces;

namespace CsSolutionManger.Console.Models;

public class Solution : ISolution
{
    private readonly SolutionCliApi _solutionCliApi;

    public Solution(string solutionFullPath, DotNetCli.DotNetCli dotNetCli)
    {
        if (!File.Exists(solutionFullPath))
            throw new InvalidOperationException("Solution file not found at path");

        FullPath = solutionFullPath;
        Id = Guid.NewGuid();

        _solutionCliApi = dotNetCli.Solution(this);
    }

    public Guid Id { get; set; }
    public string FullPath { get; }
    public string Directory => System.IO.Directory.GetParent(FullPath)?.FullName ?? string.Empty;
    public string Name => Path.GetFileName(FullPath);

    public Task<List<Project>> Projects
        => _solutionCliApi.Projects.Get();

    public Task AddProject(Project project)
        => _solutionCliApi.Projects.Add(project, Directory);

    public Task RemoveProject(Project project)
        => _solutionCliApi.Projects.Remove(project);
}