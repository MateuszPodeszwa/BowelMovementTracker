namespace BowelMovementTracker.Data.Services.UserSecurity.PasswordService;

public interface IPasswordStrategy // Strategy
{
    string Hash(string plainText);
    bool Verify(string hash, string plainText);
}