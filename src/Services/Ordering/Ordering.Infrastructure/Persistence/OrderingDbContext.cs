using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Common;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderingDbContext: DbContext
    {
        public OrderingDbContext
            (DbContextOptions<OrderingDbContext> options)
            :base(options)
        {}

        public DbSet<Order> Orders { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var item in ChangeTracker.Entries<EntityBase>())
            {
               
                switch (item.State)
                {
                    case EntityState.Modified:
                        item.Entity.ModifiedDate = DateTime.Now;
                        item.Entity.ModifiedBy = "pp";
                        break;
                    case EntityState.Added:
                        item.Entity.CreatedDate = DateTime.Now;
                        item.Entity.CreatedBy = "pp";
                        break;
                    default:
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
