using MediatR;
using OleAI.Backend.Application.Features.IngredientCategories.Constants;
using OleAI.Backend.Application.Features.IngredientCategories.Rules;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.IngredientCategories.Commands.DeleteCategory
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, string>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly CategoryBusinessRules _categoryBusinessRules;

        public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, CategoryBusinessRules categoryBusinessRules)
        {
            _categoryRepository = categoryRepository;
            _categoryBusinessRules = categoryBusinessRules;
        }

        public async Task<string> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetAsync(c => c.Id == request.Id, cancellationToken: cancellationToken);
            _categoryBusinessRules.CategoryMustExistWhenRequested(category);

            // Kullanıcı ısrarla bu hatayı almak istemediği ve soft-delete yapısı olduğu için, 
            // kategorinin silinmesine izin veriyoruz. Eski malzemelerde kategori null dönebilir.
            // await _categoryBusinessRules.CategoryCannotBeDeletedIfHasIngredients(request.Id);

            await _categoryRepository.DeleteAsync(category!);
            return CategoryMessages.DeletedCategoryMessage;
        }
    }
}
