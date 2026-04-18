using MediatR;

namespace OleAI.Backend.Application.Features.ShoppingList.Commands.BulkMarkAsPurchased;

public class BulkMarkAsPurchasedCommand : IRequest<string>
{
    public Guid UserId { get; set; }            // Controller'da token'dan set edilir
    public IReadOnlyList<Guid> ItemIds { get; set; } = [];
}
