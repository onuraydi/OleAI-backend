using MediatR;

namespace OleAI.Backend.Application.Features.Recipes.Commands.ApproveRecipe;

public class ApproveRecipeCommand : IRequest<string>
{
    [System.Text.Json.Serialization.JsonIgnore]
    public Guid Id { get; set; }

    public string? EditedDescription { get; set; }
    public List<EditedStepDto>? EditedSteps { get; set; }
}

public class EditedStepDto
{
    public int StepNumber { get; set; }
    public string Instruction { get; set; }
}
