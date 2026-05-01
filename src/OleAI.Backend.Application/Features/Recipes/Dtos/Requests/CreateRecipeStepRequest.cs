using Microsoft.AspNetCore.Http;

namespace OleAI.Backend.Application.Features.Recipes.Dtos.Requests
{
    public class CreateRecipeStepRequest
    {
        public int StepNumber { get; set; }
        public string Instruction { get; set; } = default!;
        public IFormFile? StepImage { get; set; } // Opsiyonel adım fotoğrafı
    }
}
