using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IGenericRepo<TEntity> where TEntity : class
    {
        Task<TEntity> FindByField(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes);
        Task<List<TEntity>> FindListByField(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes);
        Task<List<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity> AddAsync(TEntity entity);
        Task AddRangeAsync(List<TEntity> entities);

        void Update(TEntity entity);
        void UpdateRange(List<TEntity> entities);

        void Remove(TEntity entity);
        void RemoveRange(List<TEntity> entities);
    }
}
