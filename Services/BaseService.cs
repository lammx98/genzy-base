using Microsoft.EntityFrameworkCore;

namespace Genzy.Base.Services
{
    public class BaseService<TContext, T> : IBaseService<T>
        where TContext : DbContext
        where T : class
    {
        protected readonly TContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseService(TContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.ToListAsync();

        public virtual async Task<T?> GetByIdAsync(Guid id)
            => await _dbSet.FindAsync(id);

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}