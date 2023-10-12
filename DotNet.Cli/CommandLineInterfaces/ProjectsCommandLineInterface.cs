using DotNet.Cli.VisualStudio;

namespace DotNet.Cli.CommandLineInterfaces;

public class ProjectsCommandLineInterface<TVisualStudioObject> : CommandLineInterface, IProjectsCommandLineInterface
    where TVisualStudioObject : IVisualStudioObject
{
    private readonly TVisualStudioObject _vsObject;
    private readonly string _parentCommand;

    public ProjectsCommandLineInterface(TVisualStudioObject vsObject, string parentCommand)
    {
        _vsObject = vsObject;
        _parentCommand = parentCommand;
    }

    public PackagesCommandLineInterface Packages(Project project) => new(project);

    public ProjectsCommandLineInterface<IProject> Projects(Project project) => new(project, Command);

    public async Task<List<Project>> Get()
    {
        Command = typeof(TVisualStudioObject) == typeof(ISolution)
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

public interface IProjectsCommandLineInterface
{
    PackagesCommandLineInterface Packages(Project project);
    ProjectsCommandLineInterface<IProject> Projects(Project project);
    Task<List<Project>> Get();
    Task Add(Project project, string solutionDirectory);
    Task Remove(Project project);
    string Command { get; }
}