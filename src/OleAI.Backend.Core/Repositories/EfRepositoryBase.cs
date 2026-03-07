using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using OleAI.Backend.Core.Repositories.Dynamic;
using OleAI.Backend.Core.Repositories.Pagination;

namespace OleAI.Backend.Core.Repositories
{
    public class EfRepositoryBase<TEntity, TId> : IAsyncRepository<TEntity, TId>
        where TEntity : Entity<TId>
    {
        protected readonly DbContext _context;

        public EfRepositoryBase(DbContext context)
        {
            _context = context;
        }

        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<TEntity>().AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            await _context.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entities;
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> queryable = _context.Set<TEntity>();

            if (!enableTracking)
                queryable = queryable.AsNoTracking();

            if (withDeleted)
                queryable = queryable.Where(x => x.DeletedDate == null);

            if (predicate != null)
                queryable = queryable.Where(predicate);

            return await queryable.AnyAsync(cancellationToken);
        }

        public async Task<TEntity> DeleteAsync(TEntity entity, bool permanent = false, CancellationToken cancellationToken = default)
        {
            if (!permanent)
            {
                entity.DeletedDate = DateTime.UtcNow;
                _context.Set<TEntity>().Update(entity);
            }
            else
            {
                _context.Set<TEntity>().Remove(entity);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<IEnumerable<TEntity>> DeleteRangeAsync(IEnumerable<TEntity> entities, bool permanent = false, CancellationToken cancellationToken = default)
        {
            if (!permanent)
            {
                foreach (var entity in entities)
                {
                    entity.DeletedDate = DateTime.UtcNow;
                }
                _context.Set<TEntity>().UpdateRange(entities);
            }
            else
            {
                _context.Set<TEntity>().RemoveRange(entities);
            }
            await _context.SaveChangesAsync(cancellationToken);
            return entities;
        }

        public async Task<int> ExecuteDeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            int deletedRowCount = await _context.Set<TEntity>()
                .Where(predicate)
                .ExecuteDeleteAsync(cancellationToken);

            return deletedRowCount;
        }

        public async Task<int> ExecuteUpdateAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> setPropertyCalls, CancellationToken cancellationToken = default)
        {
            int UpdatedRowCount = await _context.Set<TEntity>()
                .Where(predicate)
                .ExecuteUpdateAsync(setPropertyCalls, cancellationToken);

            return UpdatedRowCount;
        }

        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> queryable = _context.Set<TEntity>();

            if (!enableTracking)
                queryable = queryable.AsNoTracking();

            if (!withDeleted)
                queryable = queryable.Where(x => x.DeletedDate == null);

            if (include != null)
                queryable = include(queryable);

            return await queryable.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<int> GetCountAsync(Expression<Func<TEntity, bool>>? predicate = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> queryable = _context.Set<TEntity>();

            if (!enableTracking)
                queryable = queryable.AsNoTracking();

            if (!withDeleted)
                queryable = queryable.Where(x => x.DeletedDate == null);

            if (predicate != null)
                queryable = queryable.Where(predicate);

            return await queryable.CountAsync(cancellationToken);
        }

        public async Task<Paginate<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> queryable = _context.Set<TEntity>();

            if (!enableTracking)
                queryable = queryable.AsNoTracking();

            if (!withDeleted)
                queryable = queryable.Where(x => x.DeletedDate == null);

            if (include != null)
                queryable = include(queryable);

            if (orderBy != null)
                queryable = orderBy(queryable);

            if (predicate != null)
                queryable = queryable.Where(predicate);

            var totalItems = await queryable.CountAsync(cancellationToken);
            var items = await queryable.Skip(index * size).Take(size).ToListAsync(cancellationToken);

            return new Paginate<TEntity>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = index + 1,
                PageSize = size,
                TotalPages = (int)Math.Ceiling(totalItems / (double)size)
            };
        }

        public async Task<Paginate<TEntity>> GetListByDynamicAsync(DynamicQuery dynamic, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> queryable = _context.Set<TEntity>();

            if (!enableTracking)
                queryable = queryable.AsNoTracking();

            if (include != null)
                queryable = include(queryable);

            if (withDeleted)
                queryable = queryable.IgnoreQueryFilters();

            if (predicate != null)
                queryable = queryable.Where(predicate);

            queryable = queryable.ToDynamic(dynamic);


            var totalItems = await queryable.CountAsync(cancellationToken);
            var items = await queryable.Skip(index * size).Take(size).ToListAsync(cancellationToken);

            return new Paginate<TEntity>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = index + 1,
                PageSize = size,
                TotalPages = (int)Math.Ceiling(totalItems / (double)size)
            };
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities)
            {
                entity.UpdatedDate = DateTime.UtcNow;
            }
            _context.Set<TEntity>().UpdateRange(entities);
            await _context.SaveChangesAsync(cancellationToken);
            return entities;
        }
    }
}
