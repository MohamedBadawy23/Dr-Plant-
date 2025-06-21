using Core.Enteties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Reository
{
   public interface IGenericRepository<T> where T :class
    {
       public Task<IReadOnlyList<T>> GetAllAsync();
       public Task<T> GetByIdAsync(int id);
       public Task AddAsync (T entity);
      public  Task DeleteAsync (T entity);
      public  Task UpdateAsync (T entity);
       public Task<IEnumerable<T>> SearchAsync(Expression< Func<T,bool>> predicate);
       public Task<int> SaveChangeAsync();
    }
}
