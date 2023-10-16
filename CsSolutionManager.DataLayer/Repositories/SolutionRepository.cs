
using CsSolutionManager.DataLayer.Entities;

namespace CsSolutionManager.DataLayer.Repositories;

public class SolutionRepository : GenericRepository<Solution>, ISolutionRepository
{
    public SolutionRepository(ICsSolutionManagerContext dbContext) : base(dbContext)
    {

    }
}

public interface ISolutionRepository
{
    IQueryable<Solution> Get();
    Solution? GetById(Guid id);
    Solution? GetByName(string name);
    int Add(Solution entity);
    int Remove(Solution entity);
}