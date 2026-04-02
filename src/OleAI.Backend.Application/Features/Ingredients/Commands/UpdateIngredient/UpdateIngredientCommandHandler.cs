using AutoMapper;
using MediatR;
using OleAI.Backend.Application.Features.Ingredients.Constants;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.Ingredients.Commands.UpdateIngredient
{
    public class UpdateIngredientCommandHandler : IRequestHandler<UpdateIngredientCommand, string>
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;
        private readonly IngredientBusinessRules _ingredientBusinessRules;

        public UpdateIngredientCommandHandler(IIngredientRepository ingredientRepository, IMapper mapper, IngredientBusinessRules ingredientBusinessRules)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
            _ingredientBusinessRules = ingredientBusinessRules;
        }

        public async Task<string> Handle(UpdateIngredientCommand command, CancellationToken cancellationToken)
        {
            var ingredient = await _ingredientRepository.GetAsync(i => i.Id == command.Request.Id, cancellationToken: cancellationToken);
            _ingredientBusinessRules.IngredientMustExistWhenRequested(ingredient);

            await _ingredientBusinessRules.IngredientNameCannotBeDuplicatedWhenInserted(command.Request.Name);

            _mapper.Map(command.Request, ingredient);
            await _ingredientRepository.UpdateAsync(ingredient!);

            return IngredientMessages.IngredientUpdatedMessage;
        }
    }
}
