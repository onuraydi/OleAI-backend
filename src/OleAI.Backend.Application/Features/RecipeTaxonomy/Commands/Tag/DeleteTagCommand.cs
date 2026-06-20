using MediatR;

namespace OleAI.Backend.Application.Features.RecipeTaxonomy.Commands.Tag;

public class DeleteTagCommand : IRequest<string>
{
    public int Id { get; set; }
}
