using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using OleAI.Backend.Core.Repositories.Dynamic;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;

namespace OleAI.Backend.Core.Repositories
{
    public class EfRepositoryBase<TEntity, TId>(DbContext context) : IAsyncRepository<TEntity, TId>
        where TEntity : Entity<TId>
    {
        protected readonly DbContext _context = context;

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

            if (!withDeleted)
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

        public async Task<Paginate<TEntity>> GetListAsync(QueryOptions<TEntity> options, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> queryable = _context.Set<TEntity>();

            if (!options.EnableTracking)
                queryable = queryable.AsNoTracking();

            if (options.Include != null)
                queryable = options.Include.Invoke(queryable);

            if (options.WithDeleted)
                queryable = queryable.IgnoreQueryFilters();

            if (options.Predicate != null)
                queryable = queryable.Where(options.Predicate);

            if (options.OrderBy != null)
            {
                return await options.OrderBy.Invoke(queryable)
                                    .ToPaginateAsync(options.Index, options.Size, cancellationToken);
            }

            return await queryable.ToPaginateAsync(options.Index, options.Size, cancellationToken);
        }

        public async Task<Paginate<TEntity>> GetListByDynamicAsync(DynamicQueryOptions<TEntity> options, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> queryable = _context.Set<TEntity>();

            if (!options.EnableTracking)
                queryable = queryable.AsNoTracking();

            if (options.Include != null)
                queryable = options.Include.Invoke(queryable);

            if (options.WithDeleted)
                queryable = queryable.IgnoreQueryFilters();

            if (options.Predicate != null)
                queryable = queryable.Where(options.Predicate);

            queryable = queryable.ToDynamic(options.Dynamic);

            return await queryable.ToPaginateAsync(options.Index, options.Size, cancellationToken);
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
