using Microsoft.AspNetCore.Identity;

namespace BowelMovementTracker.Data.Services.PasswordService;

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<object> _hasher = new();

    public string HashPassword(string plainTextPassword) => _hasher.HashPassword(null!, plainTextPassword);
    public bool VerifyPassword(string hashedPasswordFromDb, string plainTextPassword)
    {
        try
        {
            var result = _hasher.VerifyHashedPassword(null!, hashedPasswordFromDb, plainTextPassword);
            return result != PasswordVerificationResult.Failed;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}