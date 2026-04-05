using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Features.Auth.Dtos.Responses;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;

namespace OleAI.Backend.Application.Features.Auth.Commands.UnlockUser
{
    public class UnlockUserCommandHandler : IRequestHandler<UnlockUserCommand, UnlockedUserResponse>
    {
        private readonly IUserRepository _userRepository;

        public UnlockUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UnlockedUserResponse> Handle(UnlockUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(u => u.Id == command.Request.UserId, cancellationToken: cancellationToken);

            if (user == null)
            {
                throw new BusinessException(AuthMessages.TargetUserNotFound);
            }

            user.IsActive = true;
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;

            await _userRepository.UpdateAsync(user);

            return new UnlockedUserResponse
            {
                Message = "Kullanıcı hesabı kilidi başarıyla kaldırıldı."
            };
        }
    }
}
