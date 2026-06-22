using AutoMapper;
using MediatR;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Ingredients.Commands.CreateIngredient
{
    public class CreateIngredientCommandHandler : IRequestHandler<CreateIngredientCommand, CreatedIngredientResponse>
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;
        private readonly IngredientBusinessRules _businessRules;
        private readonly ICacheService _cacheService;

        public CreateIngredientCommandHandler(IIngredientRepository ingredientRepository, IMapper mapper, IngredientBusinessRules businessRules, ICacheService cacheService)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
            _businessRules = businessRules;
            _cacheService = cacheService;
        }
        public async Task<CreatedIngredientResponse> Handle(CreateIngredientCommand command, CancellationToken cancellationToken)
        {
            await _businessRules.IngredientNameCannotBeDuplicatedWhenInserted(command.Request.Name);
            var ingredient = _mapper.Map<Ingredient>(command.Request);
            ingredient.SetPolarizing(command.Request.IsPolarizing);
            ingredient.SetEstimatedShelfLifeInDays(command.Request.EstimatedShelfLifeInDays);
            ingredient.SetDefaultExpirationMode(command.Request.DefaultExpirationMode);
            ingredient.SetStaple(command.Request.IsStaple);
            var created = await _ingredientRepository.AddAsync(ingredient);
            await _cacheService.RemoveAsync("Pantry:AllIngredientsCatalog:v4");
            var response = _mapper.Map<CreatedIngredientResponse>(created);
            return response;
        }
    }
}
