using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace Ordering.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder, int retry = 10) where TContext : DbContext
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            var context = services.GetService<TContext>();
            var logger = services.GetRequiredService<ILogger<TContext>>();

            logger.LogInformation("Migrating mssql database.");

            try
            {
                InvokeSeeder(seeder, context, services);

                logger.LogInformation("Migrated Successfully.");
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Migration Failed.");

                if (retry-- > 0)
                {
                    logger.LogWarning("Attempts remaining {0}", retry);
                    Thread.Sleep(2000);

                    logger.LogWarning("Trying the Migration again...");
                    MigrateDatabase(host, seeder, retry);
                }
                else
                {
                    logger.LogError("Reached Maximum Attempts.");
                }
            }

            return host;
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services)
            where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context, services);
        }
    }
}