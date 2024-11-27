
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic; 
using System.Linq.Expressions;
using System.Threading.Tasks;
using Task_Flow.Core.Abstract;
using Task_Flow.Core.DataAccess;

namespace TaskFlow.Core.DataAccess.EntityFramework
{
    public class EFEntityBaseRepository<TContext, TEntity> : IEntityRepository<TEntity>
        where TEntity : class, IEntity, new()
        where TContext : DbContext

    {
        private readonly TContext _context;

        public EFEntityBaseRepository(TContext context)
        {
            _context = context;
        }

        public async Task Add(TEntity entity)
        {
            var addedEntity = _context.Entry(entity);
            addedEntity.State=EntityState.Added;
            await _context.SaveChangesAsync();
        }

        public async Task Delete(TEntity entity)
        {
            var deletedEntity = _context.Entry(entity);
            deletedEntity.State = EntityState.Deleted;
            await _context.SaveChangesAsync();
        }

        public async Task<TEntity> GetById(Expression<Func<TEntity, bool>> filter)
        {
            return await _context.Set<TEntity>().SingleOrDefaultAsync(filter);
        }

        public async Task<List<TEntity>> GetAll(Expression<Func<TEntity, bool>> filter )
        {
            return filter == null ?
                await _context.Set<TEntity>().ToListAsync() :
                await _context.Set<TEntity>().Where(filter).ToListAsync();
        }

        public async Task Update(TEntity entity)
        {
            var updatedEntity = _context.Entry(entity);
            updatedEntity.State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
