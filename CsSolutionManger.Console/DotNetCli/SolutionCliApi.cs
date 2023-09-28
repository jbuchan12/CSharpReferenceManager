using CsSolutionManger.Console.Interfaces;

namespace CsSolutionManger.Console.DotNetCli;

public class SolutionCliApi : CliApi
{
    public SolutionCliApi(ISolution solution, string parentCommand)
    {
        Command = $"{parentCommand} sln";

        Projects = new ProjectsCliApi<ISolution>(solution, Command);
    }

    public ProjectsCliApi<ISolution> Projects { get; }
}