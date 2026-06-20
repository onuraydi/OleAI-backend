using MediatR;

namespace OleAI.Backend.Application.Features.RecipeTaxonomy.Commands.Tag;

public class UpdateTagCommand : IRequest<string>
{
    public int Id { get; set; }
    public string Name { get; set; }
}
