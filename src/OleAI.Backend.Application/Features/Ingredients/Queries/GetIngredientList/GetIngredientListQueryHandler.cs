using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Ingredients.Queries.GetIngredientList
{
    public class GetIngredientListQueryHandler : IRequestHandler<GetIngredientListQuery, Paginate<IngredientResponse>>
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;

        public GetIngredientListQueryHandler(IIngredientRepository ingredientRepository, IMapper mapper)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
        }

        public async Task<Paginate<IngredientResponse>> Handle(GetIngredientListQuery request, CancellationToken cancellationToken)
        {
            QueryOptions<Ingredient> query = new QueryOptions<Ingredient>();
            query.Include = i => i.Include(x => x.Category);
            query.Index = request.Page;
            query.Size = request.Size;
            query.EnableTracking = false;

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var keyword = request.Keyword.ToLower();
                query.Predicate = i => i.Name.ToLower().Contains(keyword) ||
                                       (i.Category != null && i.Category.Name.ToLower().Contains(keyword));
            }

            var ingredients = await _ingredientRepository.GetListAsync(query, cancellationToken: cancellationToken);

            return _mapper.Map<Paginate<IngredientResponse>>(ingredients);
        }
    }
}
