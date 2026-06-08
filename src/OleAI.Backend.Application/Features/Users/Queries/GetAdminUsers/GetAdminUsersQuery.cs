using System.Collections.Generic;
using MediatR;
using OleAI.Backend.Application.Features.Users.Dtos.Responses;

namespace OleAI.Backend.Application.Features.Users.Queries.GetAdminUsers
{
    public class GetAdminUsersQuery : IRequest<List<AdminUserListItemDto>>
    {
    }
}
