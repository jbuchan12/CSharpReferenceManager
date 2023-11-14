using DotNet.Cli.VisualStudio;

namespace DotNet.Cli.CommandLineInterfaces;

public class DotNetCommandLineInterface : CommandLineInterface
{
    public SolutionCommandLineInterface Solution(ISolution solution)
        => new(solution, Command);

    public async Task<string> RunDotnetCommand(string workingDirectory, bool isInteractive = false)
        => await RunCommand("Dotnet", workingDirectory, isInteractive);

    public static List<Project> ParseCommandOutput<TVisualStudioObject>(string output, string directory, ProjectsCommandLineInterface<TVisualStudioObject> commandLineInterface)
        where TVisualStudioObject : IVisualStudioObject
        => output.Split("\r\n")
            .Where(x => x.EndsWith(".csproj"))
            .Select(x => new Project(x, directory, commandLineInterface))
            .ToList();
}

public class DotNetCliErrorException : Exception
{
    public string CommandLineError { get; }

    public DotNetCliErrorException(string commandLineError)
    {
        CommandLineError = commandLineError;
    }
}

public class DotNetPackageReferenceReadException : DotNetCliErrorException
{
    public DotNetPackageReferenceReadException(string commandLineError) : base(commandLineError)
    {

    }
}