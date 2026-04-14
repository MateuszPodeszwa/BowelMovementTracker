namespace BowelMovementTracker.Data.Services.DatabaseRepositoryService.UserRepository;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetUserWithDiaryAsync(int id);
}