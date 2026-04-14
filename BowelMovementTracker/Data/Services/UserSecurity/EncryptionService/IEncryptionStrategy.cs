namespace BowelMovementTracker.Data.Services.UserSecurity.EncryptionService;

public interface IEncryptionStrategy // Strategy
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}