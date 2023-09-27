namespace CsSolutionManger.Console.DotNetCli;

public class SolutionCliApi : CliApi
{
    private readonly ISolution _solution;

    public SolutionCliApi(ISolution solution, string parentCommand)
    {
        _solution = solution;
        Command = $"{parentCommand} sln";

        Projects = new ProjectsCliApi(solution, Command);
    }

    public ProjectsCliApi Projects { get; }

}