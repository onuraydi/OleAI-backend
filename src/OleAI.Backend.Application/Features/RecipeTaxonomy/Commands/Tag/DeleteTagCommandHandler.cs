using MediatR;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.RecipeTaxonomy.Commands.Tag;

public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand, string>
{
    private readonly ITagRepository _tagRepository;

    public DeleteTagCommandHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<string> Handle(DeleteTagCommand command, CancellationToken cancellationToken)
    {
        var tag = await _tagRepository.GetAsync(t => t.Id == command.Id, cancellationToken: cancellationToken);
        if (tag == null) throw new Exception("Etiket bulunamadı.");

        await _tagRepository.DeleteAsync(tag);

        return "Etiket başarıyla silindi.";
    }
}
