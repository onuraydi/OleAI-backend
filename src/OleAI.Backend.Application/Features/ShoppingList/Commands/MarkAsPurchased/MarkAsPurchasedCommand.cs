using MediatR;

namespace OleAI.Backend.Application.Features.ShoppingList.Commands.MarkAsPurchased;

public class MarkAsPurchasedCommand : IRequest<string>
{
    public Guid ItemId { get; set; }
    public Guid UserId { get; set; }    // Controller'da token'dan set edilir
}
