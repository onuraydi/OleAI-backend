using Microsoft.AspNetCore.Http;

namespace OleAI.Backend.Application.Features.Recipes.Dtos.Requests;

public class UpdateRecipeStepRequest
{
    public int StepNumber { get; set; }
    public string Instruction { get; set; }
    public IFormFile? NewStepImage { get; set; }
}
