using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderSeeding
    {
        public static async Task SeedAsync(OrderingDbContext ordersContext, ILogger<OrderSeeding> logger)
        {
            if (!ordersContext.Orders.Any())
            {
                await ordersContext.Orders.AddRangeAsync(GetPredefinedOrders());
                await ordersContext.SaveChangesAsync();
                logger.LogInformation($"Orders data base with context '{typeof(OrderingDbContext).Name}' successfully seeded.");
            }
        }

        private static IEnumerable<Order> GetPredefinedOrders()
        {
            return new List<Order>() { 
                new (){ 
                    FirstName="Plamen", 
                    LastName="Pentchev", 
                    UserName="Plamen Pentchev",
                    EmailAddress="plamenpentchev@yahoo.com", 
                    AddressLine="Hofheim am Taunus",
                    Country="Deutschland",
                    TotalPrice=150
                }
            };

        }
    }
}
