using AutoMapper;
using MediatR;
using OleAI.Backend.Application.Features.IngredientCategories.Constants;
using OleAI.Backend.Application.Features.IngredientCategories.Rules;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.IngredientCategories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, string>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly CategoryBusinessRules _categoryBusinessRules;

        public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IMapper mapper, CategoryBusinessRules categoryBusinessRules)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _categoryBusinessRules = categoryBusinessRules;
        }

        public async Task<string> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetAsync(c => c.Id == command.Request.Id, cancellationToken: cancellationToken);
            _categoryBusinessRules.CategoryMustExistWhenRequested(category);

            await _categoryBusinessRules.CategoryNameCannotBeDuplicatedWhenUpdated(command.Request.Id, command.Request.Name);

            _mapper.Map(command.Request, category);
            await _categoryRepository.UpdateAsync(category!);

            return CategoryMessages.UpdatedCategoryMessage;
        }
    }
}
