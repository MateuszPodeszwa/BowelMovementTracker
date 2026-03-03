namespace BowelMovementTracker.Data.Services.PasswordService;

public interface IPasswordService
{
    public string HashPassword(string plainTextPassword);
    public bool VerifyPassword(string hashedPasswordFromDb, string plainTextPassword);
}