using MediatR;

namespace OleAI.Backend.Application.Features.RecipeTaxonomy.Commands.Tag;

public class CreateTagCommand : IRequest<string>
{
    public string Name { get; set; }
}
