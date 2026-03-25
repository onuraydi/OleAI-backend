using Microsoft.EntityFrameworkCore;

namespace OleAI.Backend.Core.Repositories.Pagination
{
    public static class IQueryablePaginateExtensions
    {
        public static async Task<Paginate<T>> ToPaginateAsync<T>(
            this IQueryable<T> source,
            int index,
            int size,
            CancellationToken cancellationToken = default)
        {
            int count = await source.CountAsync(cancellationToken).ConfigureAwait(false);

            List<T> items = await source.Skip(index * size)
                                        .Take(size)
                                        .ToListAsync(cancellationToken).ConfigureAwait(false);

            return new Paginate<T>
            {
                PageNumber = index + 1,
                PageSize = size,
                TotalItems = count,
                Items = items,
                TotalPages = (int)Math.Ceiling(count / (double)size)
            };
        }
    }
}
