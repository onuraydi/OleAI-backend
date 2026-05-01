namespace OleAI.Backend.Application.Features.Recipes.Dtos.Responses
{
    public class RecipeStepResponse
    {
        public int StepNumber { get; set; }
        public string Instruction { get; set; } = string.Empty;
    }
}
