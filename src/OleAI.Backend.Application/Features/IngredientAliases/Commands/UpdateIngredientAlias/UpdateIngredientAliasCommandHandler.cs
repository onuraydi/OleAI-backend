using AutoMapper;
using MediatR;
using OleAI.Backend.Application.Features.IngredientAliases.Constants;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Responses;
using OleAI.Backend.Application.Features.IngredientAliases.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.IngredientAliases.Commands.UpdateIngredientAlias
{
    public class UpdateIngredientAliasCommandHandler : IRequestHandler<UpdateIngredientAliasCommand, UpdatedIngredientAliasResponse>
    {
        private readonly IIngredientAliasRepository _ingredientAliasRepository;
        private readonly IMapper _mapper;
        private readonly IngredientAliasBusinessRules _ingredientAliasBusinessRules;

        public UpdateIngredientAliasCommandHandler(IIngredientAliasRepository ingredientAliasRepository, IMapper mapper, IngredientAliasBusinessRules ingredientAliasBusinessRules)
        {
            _ingredientAliasRepository = ingredientAliasRepository;
            _mapper = mapper;
            _ingredientAliasBusinessRules = ingredientAliasBusinessRules;
        }

        public async Task<UpdatedIngredientAliasResponse> Handle(UpdateIngredientAliasCommand command, CancellationToken cancellationToken)
        {
            var ingredientAlias = await _ingredientAliasRepository.GetAsync(ia => ia.Id == command.Request.Id, cancellationToken: cancellationToken);

            _ingredientAliasBusinessRules.AliasMustExistWhenRequested(ingredientAlias);
            await _ingredientAliasBusinessRules.AliasNameCannotBeDuplicated(command.Request.AliasName);

            var mappedIngredientAlias = _mapper.Map(command.Request, ingredientAlias);
            await _ingredientAliasRepository.UpdateAsync(mappedIngredientAlias!);

            var response = _mapper.Map<UpdatedIngredientAliasResponse>(mappedIngredientAlias);
            response.Message = IngredientAliasMessages.AliasUpdatedMessage;

            return response;
        }
    }
}
