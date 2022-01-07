using Ordering.Domain.Common;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Contracts.Persistence
{
    public interface IAsyncRepository<T> where T: EntityBase
    {
        #region CREATE
        public Task<T> AddAsync(T item);
        #endregion CREATE

        #region READ
        public Task<IReadOnlyList<T>> GetAllAsync();
        public Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate);
        public Task<IReadOnlyList<T>> GetAsync
            (Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy =null,
            string includeString = null,
            bool disableTracking = true);
        public Task<IReadOnlyList<T>> GetAsync
            (Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            List<Expression<Func<T, object>>> includes = null,
            bool disableTracking = true);

        public Task<T> GetByIdAsync(int id);
        #endregion READ

        #region UPDATE
        public Task UpdateAsync(T item);
        #endregion UPDATE

        #region DELETE
        public Task DeleteAsync(int id);
        public Task DeleteAsync(T item);
        #endregion DELETE
    }
}
