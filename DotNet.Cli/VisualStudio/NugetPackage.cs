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
    public Project? RegisteredProject { get; private set; }

    public void RegisterWithProject(Project matchingProject)
    {
        RegisteredProject = matchingProject;
    }
}