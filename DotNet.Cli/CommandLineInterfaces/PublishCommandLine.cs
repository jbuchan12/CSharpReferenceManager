
namespace DotNet.Cli.CommandLineInterfaces
{
    public class PublishCommandLine : CommandLineInterface, IPublishCommandLine 
    {
        public async Task<string> PublishNugetPackage(string nugetSource, string packageName, string workingDirectory)
        {
            Command = $"push -Source {nugetSource} -ApiKey az {packageName}";

            return await RunNugetCommand(workingDirectory);
        }

        protected async Task<string> RunNugetCommand(string workingDirectory, bool isInteractive = false)
            => await RunCommand("Nuget", workingDirectory, isInteractive);
    }

    public interface IPublishCommandLine
    {
        Task<string> PublishNugetPackage(string nugetSource, string packageName, string workingDirectory);
    }
}
