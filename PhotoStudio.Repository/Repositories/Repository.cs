using Microsoft.EntityFrameworkCore;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoStudio.Repository.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DbSet<TEntity> _entities;
        private readonly PhotoStudioContext _context;

        public Repository(PhotoStudioContext context)
        {
            _context = context;
            _entities = context.Set<TEntity>();
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _entities.ToListAsync();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _entities.AddAsync(entity);
        }

        public void Update(TEntity entity)
        {
            _entities.Update(entity);
        }

        public void Delete(TEntity entity)
        {
            _entities.Remove(entity);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            if (typeof(TEntity).GetProperty("Id") == null)
            {
                throw new InvalidOperationException($"Entity {typeof(TEntity).Name} does not contain a property named 'Id'.");
            }

            return await _entities.AnyAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public async Task<int> CountAsync()
        {
            return await _entities.CountAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
