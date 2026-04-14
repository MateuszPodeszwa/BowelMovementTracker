using BowelMovementTracker.Data.Services.UserSecurity.EncryptionService;
using BowelMovementTracker.Data.Services.UserSecurity.PageSecurity;
using BowelMovementTracker.Data.Services.UserSecurity.PasswordService;
using Microsoft.AspNetCore.Mvc;

namespace BowelMovementTracker.Data.Services.UserSecurity;

public class SecurityService(
    IPasswordStrategy passwordStrategy,
    IEncryptionStrategy encryptionStrategy,
    IGuard guardStrategy) : IProtect // Using DI
{
    public string Hash(string plainText) => passwordStrategy.Hash(plainText);
    public bool Verify(string hash, string plainText) => passwordStrategy.Verify(hash, plainText);
    public string Encrypt(string plainText) => encryptionStrategy.Encrypt(plainText);
    public string Decrypt(string encryptedText) => encryptionStrategy.Decrypt(encryptedText);
    public IActionResult? ValidateOrRedirect(Guid userId) => guardStrategy.ValidateOrRedirect(userId);
}