using MediatR;
using OleAI.Backend.Application.Features.Recipes.Dtos.Responses;

namespace OleAI.Backend.Application.Features.Recipes.Queries.GetRecipeDetail;

public class GetRecipeDetailQuery : IRequest<RecipeDetailResponse>
{
    public Guid Id { get; set; }
}
