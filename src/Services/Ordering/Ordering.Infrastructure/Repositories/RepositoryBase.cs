using Microsoft.EntityFrameworkCore;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Common;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Repositories
{
    public class RepositoryBase<T> : IAsyncRepository<T> where T:EntityBase
    {
        protected readonly OrderingDbContext _orderingContext;

        public RepositoryBase(OrderingDbContext orderingContext)
        {
            _orderingContext = orderingContext;
        }

        public async Task<T> AddAsync(T item)
        {
            _orderingContext.Set<T>().Add(item);
            await _orderingContext.SaveChangesAsync();
            return item;
        }

        public async Task DeleteAsync(int id) 
            => await DeleteAsync(await GetByIdAsync(id));
        

        public async Task DeleteAsync(T item)
        {
            _orderingContext.Set<T>().Remove(item);
            await _orderingContext.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
            => await _orderingContext.Set<T>().ToListAsync();
        

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
            => await _orderingContext.Set<T>().Where(predicate).ToListAsync();
        

        public async Task<IReadOnlyList<T>> GetAsync
            (Expression<Func<T, bool>> predicate = null, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
            string includeString = null, 
            bool disableTracking = true)
        {
            IQueryable<T> query = _orderingContext.Set<T>();
            if (disableTracking) query = query.AsNoTracking();
            if (!string.IsNullOrEmpty(includeString)) query = query.Include(includeString);
            if (null != predicate) query = query.Where(predicate);
            if (null != orderBy) return await orderBy(query).ToListAsync();

            return await query.ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync
            (Expression<Func<T, bool>> predicate = null, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
            List<Expression<Func<T, object>>> includes = null, 
            bool disableTracking = true)
        {
            IQueryable<T> query = _orderingContext.Set<T>();
            if (disableTracking) query = query.AsNoTracking();
            if (null != includes) query = includes.Aggregate(query, (current, include) => current.Include(include));
            if (null != predicate) query = query.Where(predicate);
            if (null != orderBy) return await orderBy(query).ToListAsync();

            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id) 
            => await _orderingContext.Set<T>().FindAsync(id);
        

        public async Task UpdateAsync(T item)
        {
            _orderingContext.Entry(item).State = EntityState.Modified;
            await _orderingContext.SaveChangesAsync();
        }
    }
}
