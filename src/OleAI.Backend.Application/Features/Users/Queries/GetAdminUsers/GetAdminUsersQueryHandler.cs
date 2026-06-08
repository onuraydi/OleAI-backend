using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Users.Dtos.Responses;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Users.Queries.GetAdminUsers
{
    public class GetAdminUsersQueryHandler : IRequestHandler<GetAdminUsersQuery, List<AdminUserListItemDto>>
    {
        private readonly IUserRepository _userRepository;

        public GetAdminUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<AdminUserListItemDto>> Handle(GetAdminUsersQuery request, CancellationToken cancellationToken)
        {
            var query = new QueryOptions<User>();
            query.Include = m => m.Include(u => u.UserOperationClaims).ThenInclude(uoc => uoc.operationClaim);
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

            return userDtos;
        }
    }
}
