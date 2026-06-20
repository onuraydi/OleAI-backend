using AutoMapper;
using MediatR;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.RecipeTaxonomy.Commands.Tag;

public class UpdateTagCommandHandler : IRequestHandler<UpdateTagCommand, string>
{
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;

    public UpdateTagCommandHandler(ITagRepository tagRepository, IMapper mapper)
    {
        _tagRepository = tagRepository;
        _mapper = mapper;
    }

    public async Task<string> Handle(UpdateTagCommand command, CancellationToken cancellationToken)
    {
        var tag = await _tagRepository.GetAsync(t => t.Id == command.Id, cancellationToken: cancellationToken);
        if (tag == null) throw new Exception("Etiket bulunamadı.");

        _mapper.Map(command, tag);
        await _tagRepository.UpdateAsync(tag);

        return "Etiket başarıyla güncellendi.";
    }
}
