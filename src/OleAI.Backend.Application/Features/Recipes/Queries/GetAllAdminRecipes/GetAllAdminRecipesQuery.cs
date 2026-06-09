using MediatR;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Recipes.Queries.GetAllAdminRecipes
{
    public class GetAllAdminRecipesQuery : IRequest<Paginate<AdminRecipeListItemDto>>
    {
        public int Page { get; set; } = 0;
        public int Size { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public RecipeStatus? Status { get; set; }
        public RecipeVisibility? Visibility { get; set; }
    }
}
