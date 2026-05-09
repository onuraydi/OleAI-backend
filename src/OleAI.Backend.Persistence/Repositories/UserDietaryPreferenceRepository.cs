using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Persistence.Contexts;

namespace OleAI.Backend.Persistence.Repositories
{
    public class UserDietaryPreferenceRepository : IUserDietaryPreferenceRepository
    {
        private readonly AppDbContext _context;

        public UserDietaryPreferenceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> ExecuteDeleteByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.UserDietaryPreferences
                .Where(udp => udp.UserId == userId)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<UserDietaryPreference> entities, CancellationToken cancellationToken = default)
        {
            await _context.UserDietaryPreferences.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
