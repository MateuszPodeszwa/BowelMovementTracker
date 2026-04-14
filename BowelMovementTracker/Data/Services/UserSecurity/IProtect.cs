using BowelMovementTracker.Data.Services.UserSecurity.EncryptionService;
using BowelMovementTracker.Data.Services.UserSecurity.PageSecurity;
using BowelMovementTracker.Data.Services.UserSecurity.PasswordService;

namespace BowelMovementTracker.Data.Services.UserSecurity;

public interface IProtect : IEncryptionStrategy, IPasswordStrategy, IGuard; // Facade