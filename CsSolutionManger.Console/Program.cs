using CsSolutionManger.Console.DotNetCli;
using CsSolutionManger.Console.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using static System.Net.Mime.MediaTypeNames;

namespace CsSolutionManger.Console;

internal class Program
{
    private static ISolution _solution = null!;
    private static void Main(string[] args)
    {
        args = new string[1];
        args[0] = @"C:\Users\Jonathan Buchan\source\repos\TidyUpService\TidyUpService.sln";
        _solution = new Solution(args[0], new DotNetCli.DotNetCli());

        IHostBuilder builder = CreateHostBuilder();
        IHost host = builder.Build();

        var csSolutionManagerService = host.Services.GetService<ICsSolutionManagerService>();
        csSolutionManagerService?.RunProjectSelection();
    }

    private static IHostBuilder CreateHostBuilder()
        => Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddLogging(l =>
                {
                    l.SetMinimumLevel(LogLevel.Trace);
                    l.AddNLog();
                });
                services.AddSingleton<ICsSolutionManagerService, CsSolutionManagerService>();
                services.AddSingleton<IConsole, Console>();
                services.AddSingleton(typeof(ISolution), _solution);
            });
}

public interface ICsSolutionManagerService
{
    void RunProjectSelection();
}

public class CsSolutionManagerService : ICsSolutionManagerService
{
    private readonly ILogger<CsSolutionManagerService> _logger;
    private readonly ISolution _solution;
    private readonly IConsole _console;

    public CsSolutionManagerService(ILogger<CsSolutionManagerService> logger, ISolution solution, IConsole console)
    {
        _logger = logger;
        _solution = solution;
        _console = console;
    }

    public void RunProjectSelection()
    {
        Project selectedProject = GetSelectedProject();
        NugetPackage selectedPackage = GetSelectedPackage(selectedProject);

        _console.WriteLine("\n***** SWITCH TO CSPROJ (Y/N?) *****\n");

        bool switchToCsproj = GetKeyBoolean(_console.ReadKey().Key);
    }

    private Project GetSelectedProject()
    {
        List<Project> projects = _solution.Projects;

        _console.WriteLine("***** SELECT PROJECT *****\n");

        for (var i = 0; i < projects.Count; i++)
        {
            Project project = projects.ElementAt(i);
            _console.WriteLine($"{project.Name} [{i}]");
        }

        int selectedProjectNumber = GetKeyNumber(_console.ReadKey().Key);
        return projects.ElementAt(selectedProjectNumber);
    }

    private NugetPackage GetSelectedPackage(Project project)
    {
        List<NugetPackage> packages = project.Packages;

        _console.WriteLine("\n***** SELECT NUGET PACKAGE *****\n");

        for (var i = 0; i < packages.Count; i++)
            _console.WriteLine($"{packages[i].Name} [{i}]");

        int selectedProjectNumber = GetKeyNumber(_console.ReadKey().Key);
        return packages.ElementAt(selectedProjectNumber);
    }

    private static bool GetKeyBoolean(ConsoleKey key) =>
        key switch
        {
            ConsoleKey.Y => true,
            ConsoleKey.N => false,
            _ => throw new NotImplementedException(),
        };

    private static int GetKeyNumber(ConsoleKey key) =>
        key switch
        {
            ConsoleKey.D0 => 0,
            ConsoleKey.D1 => 1,
            ConsoleKey.D2 => 2,
            ConsoleKey.D3 => 3,
            ConsoleKey.D4 => 4,
            ConsoleKey.D5 => 5,
            ConsoleKey.D6 => 6,
            ConsoleKey.D7 => 7,
            ConsoleKey.D8 => 8,
            ConsoleKey.D9 => 9,
            _ => -1
        };
}