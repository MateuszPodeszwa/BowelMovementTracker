using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BowelMovementTracker.Data;
using BowelMovementTracker.Data.Services.DatabaseRepositoryService;
using BowelMovementTracker.Data.Services.DatabaseRepositoryService.UserRepository;
using BowelMovementTracker.Data.Services.UserSecurity;
using BowelMovementTracker.Data.Services.UserSecurity.EncryptionService;
using BowelMovementTracker.Data.Services.UserSecurity.PageSecurity;
using BowelMovementTracker.Data.Services.UserSecurity.PasswordService;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BowelMovementTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.WebHost.UseStaticWebAssets();
            
            builder.Services.AddDbContext<BowelMovementTrackerContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("BowelMovementTrackerContext") ?? throw new InvalidOperationException("Connection string 'BowelMovementTrackerContext' not found.")));

            builder.Services.AddControllersWithViews();
            
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Login"; // Update this to your custom login route
                    options.ExpireTimeSpan = TimeSpan.FromDays(7); // How long the cookie lasts
                    options.SlidingExpiration = true; // Refresh cookie expiration on each request
                    options.Cookie.HttpOnly = true; // Prevent JavaScript access to the cookie
                    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
                        ? CookieSecurePolicy.SameAsRequest
                        : CookieSecurePolicy.Always; // Require HTTPS in production
                    options.Cookie.SameSite = SameSiteMode.Strict; // Prevent CSRF attacks
                });
            
            var encryptionKey = builder.Configuration["SecuritySettings:EncryptionKey"];

            if (string.IsNullOrEmpty(encryptionKey))
            {
                throw new InvalidOperationException("Encryption key is missing from configuration.");
            }
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<IPasswordStrategy, IdentityPasswordStrategy>();
            builder.Services.AddSingleton<IEncryptionStrategy>(new AesEncryptionStrategy(encryptionKey));
            builder.Services.AddScoped<IGuard, PageUserAuthenticator>();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IProtect, SecurityService>();

            var app = builder.Build();

            // --- AUTO-CREATE TABLES IN AZURE ---
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<BowelMovementTrackerContext>();
                    context.Database.Migrate(); // This applies your EF Core migrations automatically
                }
                catch (Exception ex)
                {
                    // Log errors if needed
                    Console.WriteLine(ex.Message);
                }
            }
            // -----------------------------------------------------

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication(); 
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}