using Microsoft.EntityFrameworkCore;
// ReSharper disable MemberCanBePrivate.Global

namespace BowelMovementTracker.Data.Services.DatabaseRepositoryService;

public abstract class Repository<T>(BowelMovementTrackerContext context, DbSet<T> dbSet) : IRepository<T>
    where T : class
{
    protected readonly BowelMovementTrackerContext Context = context;
    protected readonly DbSet<T> DbSet = dbSet;

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await DbSet.FindAsync(id);
    }

    public Task<IEnumerable<T>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(T entity)
    {
        throw new NotImplementedException();
    }

    public void Update(T entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(T entity)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync()
    {
        throw new NotImplementedException();
    }
}