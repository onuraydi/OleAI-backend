using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Persistence.Contexts;

namespace OleAI.Backend.Persistence.Repositories
{
    public class UserIngredientRepository : EfRepositoryBase<UserIngredient, Guid>, IUserIngredientRepository
    {
        public UserIngredientRepository(AppDbContext context) : base(context) { }
    }
}
