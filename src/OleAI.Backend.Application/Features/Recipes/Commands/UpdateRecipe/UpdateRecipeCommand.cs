using MediatR;
using OleAI.Backend.Application.Features.Recipes.Dtos.Requests;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Recipes.Commands.UpdateRecipe;

public class UpdateRecipeCommand : IRequest<string>
{
    public UpdateRecipeRequest Request { get; set; } = default!;
    public List<CreateRecipeIngredientRequest> Ingredients { get; set; } = new();
    public List<UpdateRecipeStepRequest> Steps { get; set; } = new();
}
