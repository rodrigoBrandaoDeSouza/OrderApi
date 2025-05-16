using Microsoft.EntityFrameworkCore;
using Orders.Domain.Interfaces;

namespace Orders.Infra
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly Data.OrdersApiDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(Data.OrdersApiDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<Guid> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            var idProperty = entity.GetType().GetProperty("Id");
            if (idProperty == null)
            {
                throw new InvalidOperationException("A entidade não possui uma propriedade Id");
            }

            return (Guid)idProperty.GetValue(entity);
        }

        public void Update(T entity) => _context.Entry(entity).State = EntityState.Modified;

        public void Remove(T entity) => _dbSet.Remove(entity);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task<(IEnumerable<T> items, int totalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            bool ascending = true)
        {
            var query = _dbSet;
            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
