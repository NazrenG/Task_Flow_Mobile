using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Core.Abstract;

namespace Task_Flow.Core.DataAccess
{
    public interface IEntityRepository<T> where T : class,IEntity, new()
    {
        Task<T> GetById(Expression<Func<T, bool>> predicate = null);
        Task<List<T>> GetAll(Expression<Func<T, bool>> predicate = null);
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);

    }
}
