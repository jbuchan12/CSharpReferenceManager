using DotNet.Cli.CommandLineInterfaces;

namespace DotNet.Cli.VisualStudio;

public class NugetPackage(string name, string version)
{
    public string Name { get; set; } = name;
    public NugetPackageVersion Version { get; } = (NugetPackageVersion)version;
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