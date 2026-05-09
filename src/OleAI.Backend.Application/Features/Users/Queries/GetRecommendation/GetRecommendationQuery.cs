using MediatR;
using OleAI.Backend.Application.Services.AiRecommendation;

namespace OleAI.Backend.Application.Features.Users.Queries.GetRecommendation
{
    public class GetRecommendationQuery : IRequest<AiRecommendationResponse>
    {
    }
}
