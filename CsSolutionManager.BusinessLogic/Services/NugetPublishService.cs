using DotNet.Cli.CommandLineInterfaces;
using DotNet.Cli.VisualStudio;

namespace CsSolutionManager.BusinessLogic.Services;
public class NugetPublishService : INugetPublishService
{
    private readonly IReferenceManagementService _referenceManagementService;
    private readonly IPublishCommandLine _publishCommandLine;

    public NugetPublishService(IReferenceManagementService referenceManagementService, IPublishCommandLine publishCommandLine)
    {
        _referenceManagementService = referenceManagementService;
        _publishCommandLine = publishCommandLine;
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

        package.Publish(
            _publishCommandLine, 
            new PackagesCommandLineInterface(package.RegisteredProject));
    }
}

public interface INugetPublishService
{
    void Publish(NugetPackage? package, ISolution? solution);
}