
using DotNet.Cli.VisualStudio;

namespace DotNet.Cli.CommandLineInterfaces
{
    public class PublishCommandLine : CommandLineInterface, IPublishCommandLine 
    {
        public async Task<string> PublishNugetPackage(string nugetSource, Project? registerProject)
        {
            if(registerProject is null)
                throw new ArgumentNullException(nameof(registerProject));

            Command = $"push -Source {nugetSource} -ApiKey az {registerProject.ReleaseName}";

            return await RunNugetCommand(registerProject.ReleaseDirectory);
        }

        protected async Task<string> RunNugetCommand(string workingDirectory, bool isInteractive = false)
            => await RunCommand("Nuget", workingDirectory, isInteractive);
    }

    public interface IPublishCommandLine
    {
        Task<string> PublishNugetPackage(string nugetSource, Project? registerProject);
    }
}
