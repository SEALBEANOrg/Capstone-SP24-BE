using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Models;
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
