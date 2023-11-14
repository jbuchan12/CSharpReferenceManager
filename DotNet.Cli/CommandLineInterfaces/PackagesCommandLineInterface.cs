using DotNet.Cli.VisualStudio;

namespace DotNet.Cli.CommandLineInterfaces;

public interface IPackagesCommandLineInterface
{
    Task<List<NugetPackage>> Get();
    Task Add(NugetPackage package);
    Task Remove(NugetPackage package);
    Task Pack(NugetPackage package);
}

public class PackagesCommandLineInterface : DotNetCommandLineInterface, IPackagesCommandLineInterface
{
    private readonly Project _project;

    public PackagesCommandLineInterface(Project project)
    {
        _project = project;
    }

    public async Task<List<NugetPackage>> Get()
    {
        Command = $"list {_project.Name} package";

        var output = string.Empty;

        try
        {
            output = await RunDotnetCommand(_project.Directory);
        }
        catch (DotNetPackageReferenceReadException)
        {
            output = await RetryCommand();
        }

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
        Command = $"add {_project.Name} package {package.Name} --version {package.Version} --no-restore";
        return RunDotnetCommand(_project.Directory);
    }

    public Task Remove(NugetPackage package)
    {
        Command = $"remove {_project.Name} package {package.Name}";
        return RunDotnetCommand(_project.Directory);
    }

    public Task Pack(NugetPackage package)
    {
        if (package.RegisteredProject is null)
            throw new ArgumentNullException(nameof(package.RegisteredProject));

        Project project = package.RegisteredProject;

        Command = $"pack {project.Name} -p:PackageVersion=1.0.2 --configuration release";
        return RunDotnetCommand(_project.Directory);
    }

    private async Task<string> RetryCommand()
    {
        var readError = true;
        var output = string.Empty;

        while (readError)
        {
            try
            {
                output = await RunDotnetCommand(_project.Directory);
            }
            catch (DotNetPackageReferenceReadException)
            {
                continue;
            }
            
            readError = false;
        }

        return output;
    }
}