namespace BowelMovementTracker.Data.Services.DatabaseRepositoryService;

public interface IRepository<T> where T : class
{
    public Task<T?> GetByIdAsync(Guid id);
    public Task<IEnumerable<T>> GetAllAsync();
    public Task AddAsync(T entity);
    public void Update(T entity);
    public void Delete(T entity);
    public Task SaveChangesAsync();
}