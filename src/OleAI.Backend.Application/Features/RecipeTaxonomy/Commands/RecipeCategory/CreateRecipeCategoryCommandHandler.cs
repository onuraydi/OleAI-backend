using AutoMapper;
using MediatR;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.RecipeTaxonomy.Commands.RecipeCategory;

public class CreateRecipeCategoryCommandHandler : IRequestHandler<CreateRecipeCategoryCommand, string>
{
    private readonly IRecipeCategoryRepository _recipeCategoryRepository;
    private readonly IMapper _mapper;

    public CreateRecipeCategoryCommandHandler(IRecipeCategoryRepository recipeCategoryRepository, IMapper mapper)
    {
        _recipeCategoryRepository = recipeCategoryRepository;
        _mapper = mapper;
    }

    public async Task<string> Handle(CreateRecipeCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = _mapper.Map<Domain.Entities.RecipeCategory>(command);
        await _recipeCategoryRepository.AddAsync(category);
        return "Tarif kategorisi başarıyla eklendi.";
    }
}
