using AutoMapper;
using MediatR;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.RecipeTaxonomy.Commands.RecipeCategory;

public class UpdateRecipeCategoryCommandHandler : IRequestHandler<UpdateRecipeCategoryCommand, string>
{
    private readonly IRecipeCategoryRepository _recipeCategoryRepository;
    private readonly IMapper _mapper;

    public UpdateRecipeCategoryCommandHandler(IRecipeCategoryRepository recipeCategoryRepository, IMapper mapper)
    {
        _recipeCategoryRepository = recipeCategoryRepository;
        _mapper = mapper;
    }

    public async Task<string> Handle(UpdateRecipeCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await _recipeCategoryRepository.GetAsync(c => c.Id == command.Id, cancellationToken: cancellationToken);
        if (category == null) throw new Exception("Tarif kategorisi bulunamadı.");

        _mapper.Map(command, category);
        await _recipeCategoryRepository.UpdateAsync(category);

        return "Tarif kategorisi başarıyla güncellendi.";
    }
}
