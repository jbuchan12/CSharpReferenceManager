using DotNet.Cli.VisualStudio;

namespace DotNet.Cli.CommandLineInterfaces;

public class PackagesCommandLineInterface : CommandLineInterface
{
    private readonly Project _project;

    public PackagesCommandLineInterface(Project project)
    {
        _project = project;
    }

    public async Task<List<NugetPackage>> Get()
    {
        Command = $"list {_project.Name} package";
        string output = await RunDotnetCommand(_project.Directory);

        List<string> packageStrings = output.Split('>')
            .Skip(1) //First item is just general information
            .ToList();

        return packageStrings
            .Select(packageString => packageString.Split(' ')
                .Where(x => x.Length > 0))
            .Select(values => new NugetPackage(values.ElementAt(0), values.ElementAt(1)))
            .ToList();
    }

    public Task Add(NugetPackage package)
    {
        Command = $"remove {_project.Name} package {package.Name}";
        return RunDotnetCommand(_project.Directory);
    }

    public Task Remove(NugetPackage package)
    {
        Command = $"remove {_project.Name} package {package.Name}";
        return RunDotnetCommand(_project.Directory);
    }
}