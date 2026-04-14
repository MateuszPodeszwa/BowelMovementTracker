using Microsoft.EntityFrameworkCore;

namespace BowelMovementTracker.Data.Services.DatabaseRepositoryService.UserRepository;

public class UserRepository(BowelMovementTrackerContext context, DbSet<User> dbSet) : Repository<User>(context, dbSet), IUserRepository
{
    public Task<User> GetUserWithDiaryAsync(int id)
    {
        
        throw new NotImplementedException();
    }
}