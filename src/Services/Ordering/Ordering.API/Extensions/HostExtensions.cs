using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Ordering.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext> 
            (this IHost host, Action<TContext, IServiceProvider> seeder,  int? retry = 0)
            where TContext: DbContext
        {
            int retryForAvilability = retry.Value;
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetService <TContext>();
            var logger = services.GetRequiredService<ILogger<TContext>>();
            try
            {
                logger.LogInformation($"Migrating database associated with context {typeof(TContext).Name}");
                InvokeSeeder(seeder, context, services);
                
                logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}");
            }
            catch (SqlException ex)
            {
                if (retryForAvilability < 50)
                {
                    retryForAvilability++;
                    System.Threading.Thread.Sleep(1000);
                    MigrateDatabase<TContext>(host, seeder, retryForAvilability);
                }
            }
            
            return host;
        }

        private static void InvokeSeeder<TContext>
            (Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services) 
            where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context, services);
        }
    }
}
