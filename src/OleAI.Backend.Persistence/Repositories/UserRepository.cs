using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Persistence.Contexts;

namespace OleAI.Backend.Persistence.Repositories
{
    public class UserRepository : EfRepositoryBase<User, Guid>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }
    }
}
