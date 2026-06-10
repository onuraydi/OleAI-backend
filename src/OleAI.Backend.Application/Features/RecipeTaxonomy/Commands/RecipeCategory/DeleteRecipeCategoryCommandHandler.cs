using MediatR;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.RecipeTaxonomy.Commands.RecipeCategory;

public class DeleteRecipeCategoryCommandHandler : IRequestHandler<DeleteRecipeCategoryCommand, string>
{
    private readonly IRecipeCategoryRepository _recipeCategoryRepository;

    public DeleteRecipeCategoryCommandHandler(IRecipeCategoryRepository recipeCategoryRepository)
    {
        _recipeCategoryRepository = recipeCategoryRepository;
    }

    public async Task<string> Handle(DeleteRecipeCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await _recipeCategoryRepository.GetAsync(c => c.Id == command.Id, cancellationToken: cancellationToken);
        if (category == null) throw new Exception("Tarif kategorisi bulunamadı.");

        await _recipeCategoryRepository.DeleteAsync(category);

        return "Tarif kategorisi başarıyla silindi.";
    }
}
