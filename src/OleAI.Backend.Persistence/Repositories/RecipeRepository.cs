using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Persistence.Contexts;

namespace OleAI.Backend.Persistence.Repositories;

public class RecipeRepository : EfRepositoryBase<Recipe, Guid>, IRecipeRepository
{
    public RecipeRepository(AppDbContext context) : base(context)
    {
    }
}
