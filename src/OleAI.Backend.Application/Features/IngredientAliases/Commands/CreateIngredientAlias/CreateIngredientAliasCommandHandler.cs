using AutoMapper;
using MediatR;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Responses;
using OleAI.Backend.Application.Features.IngredientAliases.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.IngredientAliases.Commands.CreateIngredientAlias
{
    public class CreateIngredientAliasCommandHandler : IRequestHandler<CreateIngredientAliasCommand, CreatedIngredientAliasResponse>
    {
        private readonly IIngredientAliasRepository _ingredientAliasRepository;
        private readonly IMapper _mapper;
        private readonly IngredientAliasBusinessRules _ingredientAliasBusinessRules;

        public CreateIngredientAliasCommandHandler(IIngredientAliasRepository ingredientAliasRepository, IMapper mapper, IngredientAliasBusinessRules ingredientAliasBusinessRules)
        {
            _ingredientAliasRepository = ingredientAliasRepository;
            _mapper = mapper;
            _ingredientAliasBusinessRules = ingredientAliasBusinessRules;
        }

        public async Task<CreatedIngredientAliasResponse> Handle(CreateIngredientAliasCommand command, CancellationToken cancellationToken)
        {
            await _ingredientAliasBusinessRules.MasterIngredientMustExist(command.Request.IngredientId);
            await _ingredientAliasBusinessRules.AliasNameCannotBeDuplicated(command.Request.AliasName);

            var IngredientAlias = _mapper.Map<IngredientAlias>(command.Request);
            var createdIngredientAlias = await _ingredientAliasRepository.AddAsync(IngredientAlias, cancellationToken);

            return _mapper.Map<CreatedIngredientAliasResponse>(createdIngredientAlias);
        }
    }
}
