using ASP.Authentication.Data;
using Core.Enteties;
using Core.Reository;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository;
using System.Linq.Expressions;

namespace Repository.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T :class
    {
        private readonly ApplicationDbContext _dbContext;
        

        public GenericRepository(ApplicationDbContext dbContext) 
        {
            _dbContext = dbContext;
             
        }

        public async Task AddAsync(T entity)
        {
             await _dbContext.AddAsync(entity);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            if(typeof(T)== typeof(ReportProblem))
                return (IReadOnlyList<T>) await _dbContext.ReportProblems.Include(P=>P.AppUser).ToListAsync();
            return await _dbContext.Set<T>().ToListAsync();
        }



        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        

         

         

        public async Task UpdateAsync(T entity)
        {
           _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

        }

       public async Task DeleteAsync(T entity)
        {
             _dbContext.Entry(entity).State= EntityState.Deleted;
            await _dbContext.SaveChangesAsync();

        }

        public async Task<IEnumerable<T>> SearchAsync(Expression< Func<T, bool>> predicate)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (typeof(T) == typeof(ReportProblem))
            {
                query = query.Include("AppUser").Include("PlantImage");
            }

            return await query.Where(predicate).ToListAsync();
        }

        public Task<int> SaveChangeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
