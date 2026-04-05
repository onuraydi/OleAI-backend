using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Persistence.Contexts;

namespace OleAI.Backend.Persistence.Repositories
{
    public class IngredientAliasRepository : EfRepositoryBase<IngredientAlias, Guid>, IIngredientAliasRepository
    {
        public IngredientAliasRepository(AppDbContext context) : base(context)
        {
        }
    }
}
