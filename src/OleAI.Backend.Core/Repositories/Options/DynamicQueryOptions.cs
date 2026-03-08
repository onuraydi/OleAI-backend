using OleAI.Backend.Core.Repositories.Dynamic;

namespace OleAI.Backend.Core.Repositories.Options
{
    public class DynamicQueryOptions<TEntity> : BaseQueryOptions<TEntity>
    {
        public DynamicQuery Dynamic { get; set; } = null!;
    }
}
