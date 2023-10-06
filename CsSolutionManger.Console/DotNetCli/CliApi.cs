using System.Diagnostics;
using CsSolutionManger.Console.Interfaces;
using CsSolutionManger.Console.Models;

namespace CsSolutionManger.Console.DotNetCli;

public abstract class CliApi
{
    public string Command { get; protected set; } = string.Empty;

    protected async Task<string> RunDotnetCommand(string workingDirectory)
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
        }) ?? throw new Exception("process is null");

        await process.WaitForExitAsync();

        string error = await process.StandardError.ReadToEndAsync();

        if (!string.IsNullOrEmpty(error))
            throw new Exception(error);

        return await process.StandardOutput.ReadToEndAsync();
    }

    protected List<Project> ParseCommandOutput<TVisualStudioObject>(string output, string directory, ProjectsCliApi<TVisualStudioObject> cliApi) 
        where TVisualStudioObject : IVisualStudioObject
        => output.Split("\r\n")
        .Where(x => x.EndsWith(".csproj"))
        .Select(x => new Project(x, directory, cliApi))
        .ToList();
}