using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data.Db
{
    public static class PrepDb
    {
        public static void PrepPopulation(WebApplication app, bool isProduction)
        {
            using (var serviceScope = app.Services.CreateScope())
            {
                var context = serviceScope.ServiceProvider?.GetService<AppDbContext>();
                if(context is null)
                    throw new NullReferenceException(nameof(context));
                SeedData(context!, isProduction);
            }
        }

        private static void SeedData(AppDbContext context, bool isProduction)
        {
            if (isProduction)
            {
                try
                {
                    Console.WriteLine("Attempting to apply migrations");
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"Could not run migrations: {ex.Message}");
                }
            }

            if (!context.Platforms.Any())
            {
                Console.WriteLine("--> Seeding Data...");

                context.Platforms.AddRange(
                    new Platform() { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
                    new Platform() { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
                    new Platform() { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
                );

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine(" --> We already have data");
            }
        }
    }
}