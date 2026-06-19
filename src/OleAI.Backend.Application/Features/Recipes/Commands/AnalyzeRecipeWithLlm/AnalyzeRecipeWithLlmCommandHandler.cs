using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Services.Llm;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;

namespace OleAI.Backend.Application.Features.Recipes.Commands.AnalyzeRecipeWithLlm
{
    public class AnalyzeRecipeWithLlmCommandHandler : IRequestHandler<AnalyzeRecipeWithLlmCommand, string>
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly ILocalLlmService _localLlmService;

        public AnalyzeRecipeWithLlmCommandHandler(IRecipeRepository recipeRepository, ILocalLlmService localLlmService)
        {
            _recipeRepository = recipeRepository;
            _localLlmService = localLlmService;
        }

        public async Task<string> Handle(AnalyzeRecipeWithLlmCommand request, CancellationToken cancellationToken)
        {
            var recipe = await _recipeRepository.GetAsync(
                predicate: r => r.Id == request.RecipeId,
                include: q => q.Include(r => r.RecipeSteps),
                enableTracking: true,
                cancellationToken: cancellationToken);

            if (recipe == null)
                throw new NotFoundException("Tarif bulunamadı.");

            // Adımları sırayla bir liste olarak al
            var stepsList = recipe.RecipeSteps.OrderBy(s => s.StepNumber).Select(s => s.Instruction).ToList();

            // Ollama'ya gönder ve JSON sonucunu al
            var llmResultJson = await _localLlmService.AnalyzeRecipeContentAsync(recipe.Description, stepsList, cancellationToken);

            // Sonucu kaydet
            recipe.SetLlmReviewResult(llmResultJson);

            await _recipeRepository.UpdateAsync(recipe);

            return "Yapay zeka analizi başarıyla tamamlandı.";
        }
    }
}
