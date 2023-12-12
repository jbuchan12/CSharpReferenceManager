using CsSolutionManager.DataLayer.Entities;
using CsSolutionManager.DataLayer.Repositories;
using DotNet.Cli.VisualStudio;
using Solution = CsSolutionManager.DataLayer.Entities.Solution;

namespace CsSolutionManager.BusinessLogic.Services;

public class ApplicationHistoryService(
    IApplicationHistoryRepository applicationHistoryRepository, 
    ISolutionRepository solutionRepository) : IApplicationHistoryService
{
    public ApplicationHistory? GetLatestHistory()
        => applicationHistoryRepository.GetLatest();

    public void UpdateSolution(ISolution solution)
    {
        Solution existingSolution = solutionRepository.GetById(solution.Id) 
             ?? new Solution
             {
                 Name = solution.Name,
                 FullPath = solution.FullPath,
                 Directory = solution.Directory,
                 Id = solution.Id
             };

        solutionRepository.Add(existingSolution);

        var newHistory = new ApplicationHistory
        {
            Id = Guid.NewGuid(),
            Solution = existingSolution,
            Date = DateTime.Now
        };

        applicationHistoryRepository.Add(newHistory);
    }
}

public interface IApplicationHistoryService
{
    ApplicationHistory? GetLatestHistory();
    void UpdateSolution(ISolution solution);
}