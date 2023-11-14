using DotNet.Cli.CommandLineInterfaces;

namespace DotNet.Cli.VisualStudio;

public class NugetPackage
{

    public NugetPackage(string name, string version)
    {
        Name = name;
        Version = version;
    }

    public string Name { get; set; }
    public string Version { get; }
    public Project? RegisteredProject { get; set; }

    public void Publish(IPublishCommandLine publishCommandLine, IPackagesCommandLineInterface packagesCommandLineInterface)
    {
        if (RegisteredProject is null)
            throw new ArgumentNullException(nameof(RegisteredProject));

        packagesCommandLineInterface.Pack(this);
    }
}