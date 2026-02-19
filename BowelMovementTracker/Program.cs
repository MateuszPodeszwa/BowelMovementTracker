using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BowelMovementTracker.Data;

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