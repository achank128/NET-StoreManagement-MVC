using System.Linq.Expressions;

namespace StoreManagement.Repositories.RepositoryBase
{
    public interface IRepositoryBase<TEntity> where TEntity : class
    {

        TEntity GetById<T>(T Id);

        IQueryable<TEntity> GetBy(Expression<Func<TEntity, bool>> predicate);

        IEnumerable<TEntity> GetAll();

        IQueryable<TEntity> GetQueryable();

        void Add(TEntity Entity);

        void AddRange(List<TEntity> Entities);

        void Update(TEntity Entity);

        void DeleteBy<T>(T Id);

        void Delete(TEntity Entity);

        void DeleteRange(List<TEntity> Entities);

        bool Save();
    }
}
