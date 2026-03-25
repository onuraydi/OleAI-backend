namespace OleAI.Backend.Core.Repositories.Options
{
    public class QueryOptions<TEntity> : BaseQueryOptions<TEntity>
    {
        public Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy { get; set; } = null;
    }
}
