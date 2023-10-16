using CsSolutionManager.DataLayer.Entities;

namespace CsSolutionManager.DataLayer.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class, IEntity 
{
    private readonly ICsSolutionManagerContext _dbContext;
    public GenericRepository(ICsSolutionManagerContext dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual IQueryable<TEntity> Get() 
        => _dbContext
            .Table<TEntity>();

    public virtual TEntity? GetById(Guid id) 
        => _dbContext
            .Table<TEntity>()
            .Find(id);

    public virtual TEntity? GetByName(string name)
        => _dbContext
            .Table<TEntity>()
            .SingleOrDefault(x => x.Name == name);

    public virtual int Add(TEntity entity)
    {
        _ = _dbContext
            .Table<TEntity>()
            .Add(entity);

        return _dbContext.SaveChanges();
    }

    public virtual int Remove(TEntity entity)
    {
        _ = _dbContext
            .Table<TEntity>()
            .Remove(entity);

        return _dbContext.SaveChanges();
    }
}

public interface IGenericRepository<TEntity> where TEntity : class, IEntity
{
    public IQueryable<TEntity> Get();
    public TEntity? GetById(Guid id);
    public TEntity? GetByName(string name);
    public int Add(TEntity entity);
    public int Remove(TEntity entity);
}