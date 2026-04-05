using AutoMapper;
using MediatR;
using OleAI.Backend.Application.Features.IngredientCategories.Constants;
using OleAI.Backend.Application.Features.IngredientCategories.Dtos.Response;
using OleAI.Backend.Application.Features.IngredientCategories.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.IngredientCategories.Commands.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CreatedCategoryResponse>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly CategoryBusinessRules _categoryBusinessRules;

        public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IMapper mapper, CategoryBusinessRules categoryBusinessRules)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _categoryBusinessRules = categoryBusinessRules;
        }

        public async Task<CreatedCategoryResponse> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            await _categoryBusinessRules.CategoryNameCannotBeDuplicatedWhenInserted(command.Request.Name);

            var mappedCategory = _mapper.Map<Category>(command.Request);
            // Rich Domain Model nedeniyle ID'yi manuel atamamız gerekiyor çünkü constructor kullandık
            mappedCategory.Id = Guid.NewGuid();

            var createdCategory = await _categoryRepository.AddAsync(mappedCategory);
            var response = _mapper.Map<CreatedCategoryResponse>(createdCategory);
            response.Message = CategoryMessages.CreatedCategoryMessage;

            return response;
        }
    }
}
