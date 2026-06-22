using AutoMapper;
using MediatR;
using OleAI.Backend.Application.Features.Ingredients.Constants;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;

namespace OleAI.Backend.Application.Features.Ingredients.Commands.UpdateIngredient
{
    public class UpdateIngredientCommandHandler : IRequestHandler<UpdateIngredientCommand, string>
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;
        private readonly IngredientBusinessRules _ingredientBusinessRules;
        private readonly ICacheService _cacheService;

        public UpdateIngredientCommandHandler(IIngredientRepository ingredientRepository, IMapper mapper, IngredientBusinessRules ingredientBusinessRules, ICacheService cacheService)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
            _ingredientBusinessRules = ingredientBusinessRules;
            _cacheService = cacheService;
        }

        public async Task<string> Handle(UpdateIngredientCommand command, CancellationToken cancellationToken)
        {
            var ingredient = await _ingredientRepository.GetAsync(i => i.Id == command.Request.Id, cancellationToken: cancellationToken);
            _ingredientBusinessRules.IngredientMustExistWhenRequested(ingredient);

            await _ingredientBusinessRules.IngredientNameCannotBeDuplicatedWhenUpdated(command.Request.Id, command.Request.Name);

            _mapper.Map(command.Request, ingredient);
            ingredient.SetPolarizing(command.Request.IsPolarizing);
            ingredient.SetEstimatedShelfLifeInDays(command.Request.EstimatedShelfLifeInDays);
            ingredient.SetDefaultExpirationMode(command.Request.DefaultExpirationMode);
            ingredient.SetStaple(command.Request.IsStaple);
            await _ingredientRepository.UpdateAsync(ingredient!);
            await _cacheService.RemoveAsync("Pantry:AllIngredientsCatalog:v4");

            return IngredientMessages.IngredientUpdatedMessage;
        }
    }
}
