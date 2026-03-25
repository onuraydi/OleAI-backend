using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace OleAI.Backend.Core.Repositories.Options
{
    public class BaseQueryOptions<TEntity>
    {
        public Expression<Func<TEntity, bool>>? Predicate { get; set; } = null;
        public Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include { get; set; } = null;
        public int Index { get; set; } = 0;
        public int Size { get; set; } = 10;
        public bool WithDeleted { get; set; } = false;
        public bool EnableTracking { get; set; } = true;
    }
}
