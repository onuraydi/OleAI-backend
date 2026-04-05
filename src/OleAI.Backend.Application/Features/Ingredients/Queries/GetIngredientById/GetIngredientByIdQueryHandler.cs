using AutoMapper;
using MediatR;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.Ingredients.Queries.GetIngredientById
{
    public class GetIngredientByIdQueryHandler : IRequestHandler<GetIngredientByIdQuery, IngredientResponse>
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;
        private readonly IngredientBusinessRules _ingredientBusinessRules;

        public GetIngredientByIdQueryHandler(IIngredientRepository ingredientRepository, IMapper mapper, IngredientBusinessRules ingredientBusinessRules)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
            _ingredientBusinessRules = ingredientBusinessRules;
        }

        public async Task<IngredientResponse> Handle(GetIngredientByIdQuery request, CancellationToken cancellationToken)
        {
            var ingredient = await _ingredientRepository.GetAsync(i => i.Id == request.Id, cancellationToken: cancellationToken);

            _ingredientBusinessRules.IngredientMustExistWhenRequested(ingredient);

            return _mapper.Map<IngredientResponse>(ingredient);
        }
    }
}
