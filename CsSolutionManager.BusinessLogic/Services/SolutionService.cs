using DotNet.Cli.CommandLineInterfaces;
using DotNet.Cli.VisualStudio;

namespace CsSolutionManager.BusinessLogic.Services;

public class SolutionService : ISolutionService
{
    private readonly DotNetCommandLineInterface _commandLineInterface = new();

    public ISolution? CurrentSolution { get; private set; }

    public void Init(string solutionPath)
    {
        CurrentSolution = new Solution(solutionPath, _commandLineInterface);
    }

    public async Task<List<Project>> GetProjects()
    {
        if(CurrentSolution is null)
            return new List<Project>();

        return await CurrentSolution.Projects;
    }
}

public interface ISolutionService
{
    public ISolution? CurrentSolution { get; }

    public void Init(string solutionPath);
    Task<List<Project>> GetProjects();
}