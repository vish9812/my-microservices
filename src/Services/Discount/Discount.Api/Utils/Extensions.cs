using Discount.Api.Configs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Threading;

namespace Discount.Api.Utils
{
    public static class Extensions
    {
        public static IHost MigrateDatabse<TContext>(this IHost host, int retry = 10)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            var dbConfig = services.GetRequiredService<IOptions<DatabaseConfig>>().Value;
            var logger = services.GetRequiredService<ILogger<TContext>>();

            logger.LogInformation("Migrating postgresql database.");

            try
            {
                using var connection = new NpgsqlConnection(dbConfig.ConnectionString);
                connection.Open();

                using var command = new NpgsqlCommand
                {
                    Connection = connection
                };

                command.CommandText = "DROP TABLE IF EXISTS Coupon";
                command.ExecuteNonQuery();

                command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY,
                                                                ProductName VARCHAR(24) NOT NULL,
                                                                Description TEXT,
                                                                Amount INT)";
                command.ExecuteNonQuery();

                command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('IPhone X', 'IPhone Discount', 150);";
                command.ExecuteNonQuery();

                command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Samsung 10', 'Samsung Discount', 100);";
                command.ExecuteNonQuery();

                logger.LogInformation("Migrated Successfully.");
            }
            catch (NpgsqlException ex)
            {
                logger.LogError(ex, "Migration Failed.");

                if (retry-- > 0)
                {
                    logger.LogWarning("Attempts remaining {0}", retry);
                    Thread.Sleep(2000);

                    logger.LogWarning("Trying the Migration again...");
                    MigrateDatabse<TContext>(host, retry);
                }
                else
                {
                    logger.LogError("Reached Maximum Attempts.");
                }
            }

            return host;
        }
    }
}