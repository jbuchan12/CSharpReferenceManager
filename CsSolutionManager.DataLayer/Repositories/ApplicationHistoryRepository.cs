using CsSolutionManager.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace CsSolutionManager.DataLayer.Repositories;

public class ApplicationHistoryRepository : GenericRepository<ApplicationHistory> , IApplicationHistoryRepository
{
    public ApplicationHistoryRepository(ICsSolutionManagerContext dbContext) : base(dbContext)
    {
        
    }

    public ApplicationHistory? GetLatest() 
        => Get()
            .Include(x => x.Solution)
            .OrderByDescending(x => x.Date)
            .FirstOrDefault();
}

public interface IApplicationHistoryRepository
{
    ApplicationHistory? GetLatest();
    int Add(ApplicationHistory solution);
}