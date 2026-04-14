using Microsoft.AspNetCore.Identity;

namespace BowelMovementTracker.Data.Services.UserSecurity.PasswordService;

public class IdentityPasswordStrategy : IPasswordStrategy
{
    private readonly PasswordHasher<object> _hasher = new();

    public string Hash(string plainText) => 
        _hasher.HashPassword(null!, plainText);

    public bool Verify(string hash, string plainText)
    {
        try
        {
            var result = _hasher.VerifyHashedPassword(null!, hash, plainText);
            return result != PasswordVerificationResult.Failed;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}