using System.Collections.Generic;
using MediatR;
using OleAI.Backend.Application.Features.Users.Dtos.Responses;
using OleAI.Backend.Core.Repositories.Pagination;

namespace OleAI.Backend.Application.Features.Users.Queries.GetAdminUsers
{
    public class GetAdminUsersQuery : IRequest<Paginate<AdminUserListItemDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
