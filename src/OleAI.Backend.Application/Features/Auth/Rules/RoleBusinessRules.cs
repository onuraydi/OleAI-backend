using System;
using System.Threading.Tasks;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;

namespace OleAI.Backend.Application.Features.Auth.Rules
{
    public class RoleBusinessRules
    {
        private readonly IUserRepository _userRepository;
        private readonly IOperationClaimRepository _operationClaimRepository;
        private readonly IUserOperationClaimRepository _userOperationClaimRepository;

        public RoleBusinessRules(
            IUserRepository userRepository,
            IOperationClaimRepository operationClaimRepository,
            IUserOperationClaimRepository userOperationClaimRepository)
        {
            _userRepository = userRepository;
            _operationClaimRepository = operationClaimRepository;
            _userOperationClaimRepository = userOperationClaimRepository;
        }

        public async Task UserShouldExistAsync(Guid userId)
        {
            var user = await _userRepository.GetAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new BusinessException(AuthMessages.TargetUserNotFound);
            }
        }

        public async Task OperationClaimShouldExistAsync(Guid operationClaimId)
        {
            var claim = await _operationClaimRepository.GetAsync(c => c.Id == operationClaimId);
            if (claim == null)
            {
                throw new BusinessException(AuthMessages.OperationClaimNotFound);
            }
        }

        public async Task UserShouldNotHaveRoleAlreadyAsync(Guid userId, Guid operationClaimId)
        {
            var userClaim = await _userOperationClaimRepository.GetAsync(uc => uc.UserId == userId && uc.OperationClaimId == operationClaimId);
            if (userClaim != null)
            {
                throw new BusinessException(AuthMessages.UserAlreadyHasRole);
            }
        }
    }
}
