using System.Diagnostics;
using CsSolutionManger.Console.Interfaces;

namespace CsSolutionManger.Console.DotNetCli;

public abstract class CliApi
{
    public string Command { get; protected set; } = string.Empty;

    protected string RunDotnetCommand(string workingDirectory)
    {
        Process? process = Process.Start(new ProcessStartInfo($"dotnet")
        {
            Arguments = Command,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            WorkingDirectory = workingDirectory,
        });

        process?.WaitForExit();

        string? error = process?.StandardError.ReadToEnd();

        if (!string.IsNullOrEmpty(error))
            throw new Exception(error);

        return process?.StandardOutput.ReadToEnd() ?? string.Empty;
    }

    protected List<Project> ParseCommandOutput<TVisualStudioObject>(string output, string directory, ProjectsCliApi<TVisualStudioObject> cliApi) 
        where TVisualStudioObject : IVisualStudioObject
        => output.Split("\r\n")
        .Where(x => x.EndsWith(".csproj"))
        .Select(x => new Project(x, directory, cliApi))
        .ToList();
}