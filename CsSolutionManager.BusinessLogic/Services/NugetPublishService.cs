using CsSolutionManager.BusinessLogic.Interfaces;
using DotNet.Cli.CommandLineInterfaces;
using DotNet.Cli.VisualStudio;

namespace CsSolutionManager.BusinessLogic.Services;

public class NugetPublishService(IReferenceManagementService referenceManagementService,
        IPublishCommandLine publishCommandLine,
        IMessageBox messageBox,
        IApplicationService applicationService) : INugetPublishService
{
    public void Publish(NugetPackage? package, ISolution? solution)
    {
        applicationService.WriteToOutputLabel("Publishing package....");

        ArgumentNullException.ThrowIfNull(package, nameof(package));
        ArgumentNullException.ThrowIfNull(solution, nameof(solution));

        if (package.RegisteredProject is null)
            referenceManagementService.RegisterProjectFileWithNugetPackage(package, solution);

        if (package.RegisteredProject is null)
            throw new NullReferenceException("Registered project cannot be null");

        applicationService.WriteToOutputLabel("Packaging nuget package.....");

        var packagesCli = new PackagesCommandLineInterface(package.RegisteredProject);
        packagesCli.Pack(package);

        if (!messageBox.AskQuestion($"Would you Like to publish {package.RegisteredProject.ReleaseName}?", "Publish package")) 
            return;

        applicationService.WriteToOutputLabel("Packaging nuget package.....");
        package.Publish(publishCommandLine);
    }
}

public interface INugetPublishService
{
    void Publish(NugetPackage? package, ISolution? solution);
}