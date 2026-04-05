using MediatR;
using OleAI.Backend.Application.Features.Pantry.Dtos.Responses;

namespace OleAI.Backend.Application.Features.Pantry.Queries.GetMyPantry
{
    public class GetMyPantryQuery : IRequest<List<PantryItemResponse>>
    {
        public Guid UserId { get; set; }
    }
}
