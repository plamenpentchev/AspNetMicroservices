using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<T>(this IHost host, int? retry = 0)
        {
            int retryForAvailability = retry.Value;
            using (var scope = host.Services.CreateScope())
            {
                var serviceCollection = scope.ServiceProvider;
                var configuration = serviceCollection.GetRequiredService<IConfiguration>();
                var logger = serviceCollection.GetRequiredService<ILogger<T>>();
                try
                {
                    logger.LogInformation("Migrating postgresql database");
                    var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                    connection.Open();

                    var command = new NpgsqlCommand
                    {
                        Connection = connection
                    };

                    // delete if iexists
                    command.CommandText = "DROP TABLE IF EXISTS Coupon";
                    command.ExecuteNonQuery();

                    //Create
                    command.CommandText = "CREATE TABLE Coupon (" +
                        "ID SERIAL PRIMARY KEY NOT NULL, " +
                        "ProductName VARCHAR(24) NOT NULL, " +
                        "Description Text, " +
                        "Amount INT)";
                    command.ExecuteNonQuery();

                    //Seed
                    command.CommandText = "INSERT INTO coupon(productname, description, amount) VALUES('IPhone X', 'IPhone discount', 150)";
                    command.ExecuteNonQuery();

                    //Seed again
                    command.CommandText = "INSERT INTO coupon(productname, description, amount) VALUES('Samsung 10', 'Samsung discount', 100)";
                    command.ExecuteNonQuery();

                    logger.LogInformation("Postgresql database migrated");
                }
                catch (NpgsqlException ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database");
                    if (retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDatabase<T>(host, retryForAvailability);
                    }
                }
            }
            return host;
        }
    }
}
