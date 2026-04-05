using AutoMapper;
using MediatR;
using OleAI.Backend.Application.Features.IngredientCategories.Dtos.Response;
using OleAI.Backend.Application.Features.IngredientCategories.Rules;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.IngredientCategories.Queries.GetCategoryById
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryResponse>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly CategoryBusinessRules _categoryBusinessRules;

        public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository, IMapper mapper, CategoryBusinessRules categoryBusinessRules)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _categoryBusinessRules = categoryBusinessRules;
        }

        public async Task<CategoryResponse> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetAsync(c => c.Id == request.Id, cancellationToken: cancellationToken);

            _categoryBusinessRules.CategoryMustExistWhenRequested(category!);

            return _mapper.Map<CategoryResponse>(category);
        }
    }
}
