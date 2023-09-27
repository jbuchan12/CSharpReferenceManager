namespace CsSolutionManger.Console.DotNetCli;

public class ProjectsCliApi : CliApi
{
    private readonly ISolution _solution;
    private readonly string _parentCommand;

    public ProjectsCliApi(ISolution solution, string parentCommand)
    {
        _solution = solution;
        _parentCommand = parentCommand;
    }

    public PackagesCliApi Packages(Project project) => new (project);

    public List<Project> Get()
    {
        Command = $"{_parentCommand} {_solution.Name} list";
        string output = RunDotnetCommand(_solution.Directory);

        return output.Split("\r\n")
            .Where(x => x.EndsWith(".csproj"))
            .Select(x => new Project(x,_solution.Directory, this))
            .ToList();
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