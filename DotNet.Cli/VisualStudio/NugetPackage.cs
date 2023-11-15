using DotNet.Cli.CommandLineInterfaces;

namespace DotNet.Cli.VisualStudio;

public class NugetPackage
{

    public NugetPackage(string name, string version)
    {
        Name = name;
        Version = (NugetPackageVersion)version;
    }

    public string Name { get; set; }
    public NugetPackageVersion Version { get; }
    public Project? RegisteredProject { get; set; }

    public void Publish(IPublishCommandLine publishCommandLine)
    {
        if (RegisteredProject is null)
            throw new NullReferenceException("Registered Project cannot be null");

        RegisteredProject.LinkedNugetPackage = this;

        publishCommandLine.PublishNugetPackage(
            "VWW-CSharp", 
            RegisteredProject);
    }

    internal void IncrementPatchVersion() 
        => Version.PatchVersion++;
}