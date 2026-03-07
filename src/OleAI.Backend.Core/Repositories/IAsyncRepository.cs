using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using OleAI.Backend.Core.Repositories.Dynamic;
using OleAI.Backend.Core.Repositories.Pagination;

namespace OleAI.Backend.Core.Repositories
{
    public interface IAsyncRepository<TEntity, TId> where TEntity : Entity<TId>
    {
        Task<TEntity?> GetAsync(
            Expression<Func<TEntity, bool>> predicate, // Expression sayesinde func'ın tüm tabloyu çekip ramde yapacağı sorgu sql'e dönüştürülerek veri tabanına gönderilir. Bu olumlu performans artışı sağlar.
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, // Join işlemini dinamik hale getirir. İlişkileri daha performanslı yönetir.
            bool withDeleted = false,
            bool enableTracking = true, // Eğer çekilen veri üzerinde güncelleme işlemi olacaksa değişiklik takibi (enableTracking) true kalmalı aksi halde false yapmak gerekli (performans için)
            CancellationToken cancellationToken = default);

        Task<Paginate<TEntity>> GetListAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            int index = 0,
            int size = 10,
            bool withDeleted = false,
            bool enableTracking = true,
            CancellationToken cancellationToken = default);

        Task<Paginate<TEntity>> GetListByDynamicAsync(
            DynamicQuery dynamic,
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            int index = 0,
            int size = 10,
            bool withDeleted = false,
            bool enableTracking = true,
            CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            bool withDeleted = false,
            bool enableTracking = true,
            CancellationToken cancellationToken = default);

        Task<int> GetCountAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            bool withDeleted = false,
            bool enableTracking = true,
            CancellationToken cancellationToken = default);

        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        Task<int> ExecuteUpdateAsync(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<SetPropertyCalls<TEntity>,
            SetPropertyCalls<TEntity>>> setPropertyCalls,
            CancellationToken cancellationToken = default);

        Task<TEntity> DeleteAsync(TEntity entity, bool permanent = false, CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> DeleteRangeAsync(IEnumerable<TEntity> entities, bool permanent = false, CancellationToken cancellationToken = default);

        // RAM'e hiçbir veri çekmeden, senin predicate'ini doğrudan DELETE FROM Table WHERE ... şekline çevirip veritabanına gönderir. Geriye kaç satırın silindiğini (int) döner.
        Task<int> ExecuteDeleteAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);
    }
}
