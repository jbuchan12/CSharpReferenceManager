using CsSolutionManager.BusinessLogic.Interfaces;
using DotNet.Cli.CommandLineInterfaces;
using DotNet.Cli.VisualStudio;

namespace CsSolutionManager.BusinessLogic.Services;
public class NugetPublishService : INugetPublishService
{
    private readonly IReferenceManagementService _referenceManagementService;
    private readonly IPublishCommandLine _publishCommandLine;
    private readonly IMessageBox _messageBox;
    private readonly IApplicationService _applicationService;

    public NugetPublishService(
        IReferenceManagementService referenceManagementService,
        IPublishCommandLine publishCommandLine, 
        IMessageBox messageBox,
        IApplicationService applicationService)
    {
        _referenceManagementService = referenceManagementService;
        _publishCommandLine = publishCommandLine;
        _messageBox = messageBox;
        _applicationService = applicationService;
    }

    public void Publish(NugetPackage? package, ISolution? solution)
    {
        _applicationService.WriteToOutputLabel("Publishing package....");

        ArgumentNullException.ThrowIfNull(package, nameof(package));
        ArgumentNullException.ThrowIfNull(solution, nameof(solution));

        if (package.RegisteredProject is null)
            _referenceManagementService.RegisterProjectFileWithNugetPackage(package, solution);

        if (package.RegisteredProject is null)
            throw new NullReferenceException("Registered project cannot be null");

        _applicationService.WriteToOutputLabel("Packaging nuget package.....");

        var packagesCli = new PackagesCommandLineInterface(package.RegisteredProject);
        packagesCli.Pack(package);

        if (!_messageBox.AskQuestion($"Would you Like to publish {package.RegisteredProject.ReleaseName}?", "Publish package")) 
            return;

        _applicationService.WriteToOutputLabel("Packaging nuget package.....");
        package.Publish(_publishCommandLine);
    }
}

public interface INugetPublishService
{
    void Publish(NugetPackage? package, ISolution? solution);
}