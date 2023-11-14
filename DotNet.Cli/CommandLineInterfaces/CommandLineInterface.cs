using System.Diagnostics;

namespace DotNet.Cli.CommandLineInterfaces;

public abstract class CommandLineInterface
{
    public string Command { get; protected set; } = string.Empty;

    protected async Task<string> RunCommand(string processName, string workingDirectory, bool isInteractive = false)
    {
        Process process = Process.Start(new ProcessStartInfo(processName)
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

    private Exception? GetCliException(string errorMessage)
    {
        if (string.IsNullOrEmpty(errorMessage))
            return null;

        return errorMessage.Contains("Unable to read a package reference from the project") 
            ? new DotNetPackageReferenceReadException(errorMessage) 
            : new DotNetCliErrorException(errorMessage);
    }
}