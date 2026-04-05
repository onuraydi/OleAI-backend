using AutoMapper;
using MediatR;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Responses;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.IngredientAliases.Queries.GetIngredientAliasById
{
    public class GetIngredientAliasByIdQueryHandler : IRequestHandler<GetIngredientAliasByIdQuery, IngredientAliasResponse>
    {
        private readonly IIngredientAliasRepository _ingredientAliasRepository;
        private readonly IMapper _mapper;

        public GetIngredientAliasByIdQueryHandler(IIngredientAliasRepository ingredientAliasRepository, IMapper mapper)
        {
            _ingredientAliasRepository = ingredientAliasRepository;
            _mapper = mapper;
        }

        public async Task<IngredientAliasResponse> Handle(GetIngredientAliasByIdQuery request, CancellationToken cancellationToken)
        {
            var ingredientAlias = await _ingredientAliasRepository.GetAsync(ia => ia.Id == request.Id, cancellationToken: cancellationToken);

            return _mapper.Map<IngredientAliasResponse>(ingredientAlias);
        }
    }
}
