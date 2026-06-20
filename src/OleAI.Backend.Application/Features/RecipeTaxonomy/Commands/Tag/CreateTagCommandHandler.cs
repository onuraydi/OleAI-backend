using AutoMapper;
using MediatR;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.RecipeTaxonomy.Commands.Tag;

public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, string>
{
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;

    public CreateTagCommandHandler(ITagRepository tagRepository, IMapper mapper)
    {
        _tagRepository = tagRepository;
        _mapper = mapper;
    }

    public async Task<string> Handle(CreateTagCommand command, CancellationToken cancellationToken)
    {
        var tag = _mapper.Map<Domain.Entities.Tag>(command);
        await _tagRepository.AddAsync(tag);
        return "Etiket başarıyla eklendi.";
    }
}
