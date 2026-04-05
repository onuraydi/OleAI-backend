using AutoMapper;
using MediatR;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Ingredients.Commands.CreateIngredient
{
    public class CreateIngredientCommandHandler : IRequestHandler<CreateIngredientCommand, CreatedIngredientResponse>
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;
        private readonly IngredientBusinessRules _businessRules;

        public CreateIngredientCommandHandler(IIngredientRepository ingredientRepository, IMapper mapper, IngredientBusinessRules businessRules)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
            _businessRules = businessRules;
        }
        public async Task<CreatedIngredientResponse> Handle(CreateIngredientCommand command, CancellationToken cancellationToken)
        {
            await _businessRules.IngredientNameCannotBeDuplicatedWhenInserted(command.Request.Name);
            var ingredient = _mapper.Map<Ingredient>(command.Request);
            var created = await _ingredientRepository.AddAsync(ingredient);
            var response = _mapper.Map<CreatedIngredientResponse>(created);
            return response;
        }
    }
}
