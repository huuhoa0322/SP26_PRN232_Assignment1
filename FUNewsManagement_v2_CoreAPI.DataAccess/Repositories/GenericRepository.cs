using Microsoft.EntityFrameworkCore;
using FUNewsManagement_v2_CoreAPI.DataAccess.Models;

namespace FUNewsManagement_v2_CoreAPI.DataAccess.Repositories
{
    /// <summary>
    /// Generic repository implementation
    /// </summary>
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        protected readonly FunewsManagementContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(FunewsManagementContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await SaveChangesAsync();
            return entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await SaveChangesAsync();
        }

        public virtual async Task<bool> ExistsAsync(object id)
        {
            var entity = await GetByIdAsync(id);
            return entity != null;
        }

        public virtual async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
