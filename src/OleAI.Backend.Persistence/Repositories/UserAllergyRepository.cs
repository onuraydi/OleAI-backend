using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Persistence.Contexts;

namespace OleAI.Backend.Persistence.Repositories
{
    public class UserAllergyRepository : IUserAllergyRepository
    {
        private readonly AppDbContext _context;

        public UserAllergyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> ExecuteDeleteByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.UserAllergies
                .Where(ua => ua.UserId == userId)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<UserAllergy> entities, CancellationToken cancellationToken = default)
        {
            await _context.UserAllergies.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
