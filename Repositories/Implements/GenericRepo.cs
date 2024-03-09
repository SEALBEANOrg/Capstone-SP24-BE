using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Repositories.Interfaces;
using Repositories.Models;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Repositories.Implements
{
    public class GenericRepo<TEntity> : IGenericRepo<TEntity> where TEntity : class
    {
        protected DbSet<TEntity> _dbSet;

        public GenericRepo(ExagenContext context)
        {
            _dbSet = context.Set<TEntity>();
        }
        public async Task<TEntity> AddAsync(TEntity entity)
        {
            var result = await _dbSet.AddAsync(entity);
            return result.Entity;
        }

        public async Task AddRangeAsync(List<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task<TEntity> FindByField(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes)
      => await includes
         .Aggregate(_dbSet!.AsQueryable(),
             (entity, property) => entity!.Include(property)).AsNoTracking()
         .Where(expression!)
          .FirstOrDefaultAsync();

        public async Task<List<TEntity>> FindListByField(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes)
       => await includes
          .Aggregate(_dbSet!.AsQueryable(),
              (entity, property) => entity.Include(property)).AsNoTracking()
          .Where(expression!)
           .ToListAsync();

        /// <summary>
        /// Gets the IEnumerable entities based on a conditionally, orderby delegate and include delegate. This method default no-tracking query.
        /// </summary>
        /// <param name="selector">Mannage type of IEnumerable class that you want to return it</param>
        /// <param name="filter">Set condition for filter</param>
        /// <param name="orderBy">For set order of elements</param>
        /// <param name="include">Set include and thenInclude with this</param>
        /// <param name="disableTracking">disable tracking if is true take query AsNoTracking default is true</param>
        /// <typeparam name="TResult">the type of IEnumerable that you want return</typeparam>
        /// <returns>Return IEnumerable TResult type</returns>
        public async Task<IEnumerable<TResult>?> FindByConditionAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                                                     Expression<Func<TEntity, bool>> filter = null,
                                                     Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                     Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                     bool disableTracking = true)
        {
            IQueryable<TEntity> query = _dbSet;

            query = disableTracking ? query.AsNoTracking() : query.AsTracking();

            if (include != null) query = include(query);

            if (filter != null) query = query.Where(filter);

            if (orderBy != null) _ = orderBy(query);

            return (query.Count() != 0) ? await query.Select(selector).ToListAsync() : null;
        }

        public async Task<List<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes) =>
          await includes
         .Aggregate(_dbSet.AsQueryable(),
             (entity, property) => entity.Include(property).IgnoreAutoIncludes())
         .ToListAsync();


        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(List<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public void Update(TEntity entity)
        {
            //_dbSet.Attach(entity);
            _dbSet.Update(entity);
        }

        public void UpdateRange(List<TEntity> entities)
        {
            //_dbSet.AttachRange(entities);
            _dbSet.UpdateRange(entities);
        }
    }
}
