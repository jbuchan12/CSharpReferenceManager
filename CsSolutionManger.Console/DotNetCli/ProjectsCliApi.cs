using System.Linq;
using CsSolutionManger.Console.Interfaces;

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

    public List<Project> Get()
    {
        Command = (typeof(TVisualStudioObject) == typeof(ISolution))
            ? $"{_parentCommand} {_vsObject.Name} list"
            : $"list {_vsObject.Name} reference";

        string output = RunDotnetCommand(_vsObject.Directory);
        return ParseCommandOutput(output, _vsObject.Directory, this);
    }

    public void Add(Project project)
    {
        Command = $"sln add {project.Name}";
        string output = RunDotnetCommand(project.Directory);
    }

    public void Remove(Project project)
    {
        Command = $"sln remove {project.Name}";
        string output = RunDotnetCommand(project.Directory);
    }
}