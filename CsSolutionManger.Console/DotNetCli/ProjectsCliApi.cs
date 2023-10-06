using System.Linq;
using CsSolutionManger.Console.Interfaces;
using CsSolutionManger.Console.Models;

namespace CsSolutionManger.Console.DotNetCli;

public class ProjectsCliApi<TVisualStudioObject> : CliApi, IProjectsCliApi 
    where TVisualStudioObject : IVisualStudioObject  
{
    private readonly TVisualStudioObject _vsObject;
    private readonly string _parentCommand;

    public ProjectsCliApi(TVisualStudioObject vsObject, string parentCommand)
    {
        _vsObject = vsObject;
        _parentCommand = parentCommand;
    }

    public PackagesCliApi Packages(Project project) => new (project);

    public ProjectsCliApi<IProject> Projects(Project project) => new (project, Command);

    public async Task<List<Project>> Get()
    {
        Command = (typeof(TVisualStudioObject) == typeof(ISolution))
            ? $"{_parentCommand} {_vsObject.Name} list"
            : $"list {_vsObject.Name} reference";

        string output = await RunDotnetCommand(_vsObject.Directory);
        return ParseCommandOutput(output, _vsObject.Directory, this);
    }

    public async Task Add(Project project, string solutionDirectory)
    {
        Command = $"sln add \"{project.Directory}\\{project.Name}\"";
        _ = await RunDotnetCommand(solutionDirectory);
    }

    public async Task Remove(Project project)
    {
        Command = $"sln remove {project.Name}";
        _ = await RunDotnetCommand(project.Directory);
    }
}