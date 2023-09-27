namespace CsSolutionManger.Console.DotNetCli;

public class PackagesCliApi : CliApi
{
    private readonly Project _project;

    public PackagesCliApi(Project project)
    {
        _project = project;
    }

    public List<NugetPackage> Get()
    {
        Command = $"list {_project.Name} package";
        string output = RunDotnetCommand(_project.Directory);

        return output.Split(">")
            .Skip(1)
            .Select(l => new string(l
                .Where(char.IsLetter).ToArray()))
            .Select(x => new NugetPackage(x))
            .ToList();
    }

    public void Add(NugetPackage project)
    {
        throw new NotImplementedException();
    }

    public void Remove(NugetPackage project)
    {
        throw new NotImplementedException();
    }
}