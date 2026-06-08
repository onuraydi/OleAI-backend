using AutoMapper;
using MediatR;
using OleAI.Backend.Application.Features.IngredientCategories.Dtos.Response;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.IngredientCategories.Queries.GetCategoryList
{
    public class GetCategoryListQueryHandler : IRequestHandler<GetCategoryListQuery, List<CategoryResponse>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public GetCategoryListQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<List<CategoryResponse>> Handle(GetCategoryListQuery request, CancellationToken cancellationToken)
        {
            // Dropdownlar için sayfalama kullanılmadı, gerekirse sayfalamalı versiyon da eklenebilir - admin panel için
            QueryOptions<Category> query = new QueryOptions<Category>();
            query.Size = 100;
            query.EnableTracking = false;
            query.WithDeleted = true;

            var categories = await _categoryRepository.GetListAsync(query, cancellationToken);

            return _mapper.Map<List<CategoryResponse>>(categories.Items);
        }
    }
}
