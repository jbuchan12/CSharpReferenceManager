using CsSolutionManager.DataLayer.Entities;
using CsSolutionManager.DataLayer.Repositories;
using DotNet.Cli.VisualStudio;
using Solution = CsSolutionManager.DataLayer.Entities.Solution;

namespace CsSolutionManager.BusinessLogic.Services
{
    public class ApplicationHistoryService : IApplicationHistoryService
    {
        private readonly IApplicationHistoryRepository _applicationHistoryRepository;
        private readonly ISolutionRepository _solutionRepository;

        public ApplicationHistoryService(IApplicationHistoryRepository applicationHistoryRepository, ISolutionRepository solutionRepository)
        {
            _applicationHistoryRepository = applicationHistoryRepository;
            _solutionRepository = solutionRepository;
        }

        public ApplicationHistory? GetLatestHistory()
            => _applicationHistoryRepository.GetLatest();

        public void UpdateSolution(ISolution solution)
        {
            Solution? existingSolution = _solutionRepository.GetById(solution.Id) 
            ?? new Solution
            {
                Name = solution.Name,
                FullPath = solution.FullPath,
                Directory = solution.Directory,
                Id = solution.Id
            };

            _solutionRepository.Add(existingSolution);

            var newHistory = new ApplicationHistory
            {
                Id = Guid.NewGuid(),
                Solution = existingSolution,
                Date = DateTime.Now
            };

            _applicationHistoryRepository.Add(newHistory);
        }
    }

    public interface IApplicationHistoryService
    {
        ApplicationHistory? GetLatestHistory();
        void UpdateSolution(ISolution solution);
    }
}
