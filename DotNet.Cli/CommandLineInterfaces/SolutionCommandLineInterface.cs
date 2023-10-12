using DotNet.Cli.VisualStudio;

namespace DotNet.Cli.CommandLineInterfaces;

public class SolutionCommandLineInterface : CommandLineInterface
{
    public SolutionCommandLineInterface(ISolution solution, string parentCommand)
    {
        Command = $"{parentCommand} sln";

        Projects = new ProjectsCommandLineInterface<ISolution>(solution, Command);
    }

    public ProjectsCommandLineInterface<ISolution> Projects { get; }
}