using AutoMapper;
using MediatR;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Responses;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.IngredientAliases.Queries.GetAliasesByIngredientId
{
    public class GetAliasesByIngredientIdQueryHandler : IRequestHandler<GetAliasesByIngredientIdQuery, List<IngredientAliasResponse>>
    {
        private readonly IIngredientAliasRepository _ingredientAliasRepository;
        private readonly IMapper _mapper;

        public GetAliasesByIngredientIdQueryHandler(IIngredientAliasRepository ingredientAliasRepository, IMapper mapper)
        {
            _ingredientAliasRepository = ingredientAliasRepository;
            _mapper = mapper;
        }

        public async Task<List<IngredientAliasResponse>> Handle(GetAliasesByIngredientIdQuery request, CancellationToken cancellationToken)
        {
            QueryOptions<IngredientAlias> query = new QueryOptions<IngredientAlias>();
            query.Predicate = ia => ia.IngredientId == request.IngredientId;
            query.EnableTracking = false;

            var ingredientAlias = await _ingredientAliasRepository.GetListAsync(query, cancellationToken);

            return _mapper.Map<List<IngredientAliasResponse>>(ingredientAlias.Items);
        }
    }
}
