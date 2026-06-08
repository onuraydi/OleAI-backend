using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Users.Dtos.Responses;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Users.Queries.GetAdminUsers
{
    public class GetAdminUsersQueryHandler : IRequestHandler<GetAdminUsersQuery, Paginate<AdminUserListItemDto>>
    {
        private readonly IUserRepository _userRepository;

        public GetAdminUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Paginate<AdminUserListItemDto>> Handle(GetAdminUsersQuery request, CancellationToken cancellationToken)
        {
            var query = new QueryOptions<User>();
            query.Include = m => m.Include(u => u.UserOperationClaims).ThenInclude(uoc => uoc.operationClaim);
            query.Index = request.PageNumber - 1; // Backend pagination is 0-indexed internally?
            query.Size = request.PageSize;

            var users = await _userRepository.GetListAsync(
                query,
                cancellationToken: cancellationToken
            );

            var userDtos = users.Items.Select(user => new AdminUserListItemDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsActive = user.IsActive,
                LockoutEnd = user.LockoutEnd,
                Roles = user.UserOperationClaims.Select(uoc => uoc.operationClaim.Name).ToList()
            }).ToList();

            return new Paginate<AdminUserListItemDto>
            {
                Items = userDtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = users.TotalPages,
                TotalItems = users.TotalItems
            };
        }
    }
}
