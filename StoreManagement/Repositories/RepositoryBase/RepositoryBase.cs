using Microsoft.EntityFrameworkCore;
using StoreManagement.Models;
using System.Linq.Expressions;

namespace StoreManagement.Repositories.RepositoryBase
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        private readonly StoreManagementContext _context;

        public RepositoryBase(StoreManagementContext context)
        {
            _context = context;
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return _context.Set<TEntity>();
        }

        public IQueryable<TEntity> GetBy(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().Where(predicate);
        }

        public virtual TEntity GetById<T>(T Id)
        {
            return _context.Set<TEntity>().Find(Id);
        }

        public IQueryable<TEntity> GetQueryable()
        {
            return _context.Set<TEntity>();
        }

        public void Add(TEntity Entity)
        {
            _context.Set<TEntity>().Add(Entity);
        }

        public void AddRange(List<TEntity> Entities)
        {
            _context.Set<TEntity>().AddRange(Entities);
        }
       
        public void DeleteBy<T>(T Id)
        {
            TEntity Entity = _context.Set<TEntity>().Find(Id);

            if (Entity != null)
            {
                _context.Set<TEntity>().Remove(Entity);
            }
        }

        public void Delete(TEntity Entity)
        {
            _context.Set<TEntity>().Remove(Entity);
        }

        public void DeleteRange(List<TEntity> Entities)
        {
            _context.Set<TEntity>().RemoveRange(Entities);
        }

        public void Update(TEntity Entity)
        {
            _context.Set<TEntity>().Update(Entity);
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0 ? true : false;
        }
    }
}
