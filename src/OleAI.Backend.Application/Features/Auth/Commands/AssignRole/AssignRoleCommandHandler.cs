using MediatR;
using Microsoft.Extensions.Options;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Features.Auth.Dtos.Responses;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Auth.Commands.AssignRole
{
    public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, AssignedRoleResponse>
    {
        private readonly IUserOperationClaimRepository _userOperationClaimRepository;
        private readonly RoleBusinessRules _roleBusinessRules;
        private readonly SecuritySettings _securitySettings;

        public AssignRoleCommandHandler(
            IUserOperationClaimRepository userOperationClaimRepository,
            RoleBusinessRules roleBusinessRules,
            IOptions<SecuritySettings> securitySettings)
        {
            _userOperationClaimRepository = userOperationClaimRepository;
            _roleBusinessRules = roleBusinessRules;
            _securitySettings = securitySettings.Value;
        }

        public async Task<AssignedRoleResponse> Handle(AssignRoleCommand command, CancellationToken cancellationToken)
        {
            // Business Exceptions will be thrown if any validation fails
            await _roleBusinessRules.UserShouldExistAsync(command.Request.UserId);
            await _roleBusinessRules.OperationClaimShouldExistAsync(_securitySettings.AdminRoleId);
            await _roleBusinessRules.UserShouldNotHaveRoleAlreadyAsync(command.Request.UserId, _securitySettings.AdminRoleId);

            var userOperationClaim = new UserOperationClaim
            {
                Id = Guid.NewGuid(),
                UserId = command.Request.UserId,
                OperationClaimId = _securitySettings.AdminRoleId,
            };

            await _userOperationClaimRepository.AddAsync(userOperationClaim, cancellationToken);

            return new AssignedRoleResponse
            {
                Id = userOperationClaim.Id,
                Message = AuthMessages.RoleAssignedSuccessfully
            };
        }
    }
}
