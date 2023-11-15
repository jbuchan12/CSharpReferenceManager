using CsSolutionManager.BusinessLogic.Interfaces;
using DotNet.Cli.CommandLineInterfaces;
using DotNet.Cli.VisualStudio;

namespace CsSolutionManager.BusinessLogic.Services;
public class NugetPublishService : INugetPublishService
{
    private readonly IReferenceManagementService _referenceManagementService;
    private readonly IPublishCommandLine _publishCommandLine;
    private readonly IMessageBox _messageBox;

    public NugetPublishService(
        IReferenceManagementService referenceManagementService,
        IPublishCommandLine publishCommandLine, 
        IMessageBox messageBox)
    {
        _referenceManagementService = referenceManagementService;
        _publishCommandLine = publishCommandLine;
        _messageBox = messageBox;
    }

    public void Publish(NugetPackage? package, ISolution? solution)
    {
        if(package is null) 
            throw new ArgumentNullException(nameof(package));

        if(solution is null) 
            throw new ArgumentNullException(nameof(solution));

        if (package.RegisteredProject is null)
            _referenceManagementService.RegisterProjectFileWithNugetPackage(package, solution);

        if (package.RegisteredProject is null)
            throw new ArgumentNullException(nameof(package.RegisteredProject));

        var packagesCli = new PackagesCommandLineInterface(package.RegisteredProject);
        packagesCli.Pack(package);
        
        if(_messageBox.AskQuestion($"Would you Like to publish {package.RegisteredProject.ReleaseName}?", "Publish package"))
            package.Publish(_publishCommandLine);
    }
}

public interface INugetPublishService
{
    void Publish(NugetPackage? package, ISolution? solution);
}