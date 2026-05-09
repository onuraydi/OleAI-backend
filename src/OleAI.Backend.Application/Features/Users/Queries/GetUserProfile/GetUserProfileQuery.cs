using MediatR;
using OleAI.Backend.Application.Features.Users.Dtos.Responses;

namespace OleAI.Backend.Application.Features.Users.Queries.GetUserProfile
{
    public class GetUserProfileQuery : IRequest<UserProfileResponse>
    {
    }
}
