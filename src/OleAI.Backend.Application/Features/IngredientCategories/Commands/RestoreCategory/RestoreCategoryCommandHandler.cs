using MediatR;
using OleAI.Backend.Application.Features.IngredientCategories.Constants;
using OleAI.Backend.Application.Features.IngredientCategories.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;

namespace OleAI.Backend.Application.Features.IngredientCategories.Commands.RestoreCategory
{
    public class RestoreCategoryCommandHandler : IRequestHandler<RestoreCategoryCommand, string>
    {
        private readonly ICategoryRepository _categoryRepository;

        public RestoreCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<string> Handle(RestoreCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetAsync(
                predicate: c => c.Id == request.Id,
                withDeleted: true,
                cancellationToken: cancellationToken);

            if (category == null)
            {
                throw new NotFoundException(CategoryMessages.CategoryNotFoundMessage);
            }

            category.DeletedDate = null;
            await _categoryRepository.UpdateAsync(category);

            return "Kategori başarıyla geri yüklendi.";
        }
    }
}
