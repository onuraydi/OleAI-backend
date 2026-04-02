using AutoMapper;
using MediatR;
using OleAI.Backend.Application.Features.IngredientAliases.Constants;
using OleAI.Backend.Application.Features.IngredientAliases.Rules;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.IngredientAliases.Commands.DeleteIngredientAlias
{
    public class DeleteIngredientAliasCommandHandler : IRequestHandler<DeleteIngredientAliasCommand, string>
    {
        private readonly IIngredientAliasRepository _ingredientAliasRepository;
        private readonly IMapper _mapper;
        private readonly IngredientAliasBusinessRules _ingredientAliasBusinessRules;

        public DeleteIngredientAliasCommandHandler(IIngredientAliasRepository ingredientAliasRepository, IMapper mapper, IngredientAliasBusinessRules ingredientAliasBusinessRules)
        {
            _ingredientAliasRepository = ingredientAliasRepository;
            _mapper = mapper;
            _ingredientAliasBusinessRules = ingredientAliasBusinessRules;
        }

        public async Task<string> Handle(DeleteIngredientAliasCommand request, CancellationToken cancellationToken)
        {
            var ingredientAlias = await _ingredientAliasRepository.GetAsync(ia => ia.Id == request.Id, cancellationToken: cancellationToken);

            _ingredientAliasBusinessRules.AliasMustExistWhenRequested(ingredientAlias);

            await _ingredientAliasRepository.DeleteAsync(ingredientAlias!);
            return IngredientAliasMessages.AliasDeletedMessage;
        }
    }
}
