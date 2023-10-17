using System.Diagnostics;
using DotNet.Cli.VisualStudio;

namespace DotNet.Cli.CommandLineInterfaces;

public abstract class CommandLineInterface
{
    public string Command { get; protected set; } = string.Empty;

    protected async Task<string> RunDotnetCommand(string workingDirectory, bool isInteractive = false)
    {
        Process? process = Process.Start(new ProcessStartInfo($"dotnet")
        {
            Arguments = Command,
            CreateNoWindow = !isInteractive,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            WorkingDirectory = workingDirectory,
        }) ?? throw new Exception("process is null");

        await process.WaitForExitAsync();

        string error = await process.StandardError.ReadToEndAsync();

        Exception? exception = GetCliException(error);
        if (exception is not null)
            throw exception;

        return await process.StandardOutput.ReadToEndAsync();
    }

    protected List<Project> ParseCommandOutput<TVisualStudioObject>(string output, string directory, ProjectsCommandLineInterface<TVisualStudioObject> commandLineInterface)
        where TVisualStudioObject : IVisualStudioObject
        => output.Split("\r\n")
        .Where(x => x.EndsWith(".csproj"))
        .Select(x => new Project(x, directory, commandLineInterface))
        .ToList();

    private Exception? GetCliException(string errorMessage)
    {
        if (string.IsNullOrEmpty(errorMessage))
            return null;

        return errorMessage.Contains("Unable to read a package reference from the project") 
            ? new DotNetPackageReferenceReadException(errorMessage) 
            : new DotNetCliErrorException(errorMessage);
    }
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