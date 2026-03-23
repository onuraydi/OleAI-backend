using MediatR;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Features.Auth.Dtos.Responses;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Security.Hashing;

namespace OleAI.Backend.Application.Features.Auth.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ChangePasswordResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthBusinessRules _authBusinessRules;

        public ChangePasswordCommandHandler(IUserRepository userRepository, AuthBusinessRules authBusinessRules)
        {
            _userRepository = userRepository;
            _authBusinessRules = authBusinessRules;
        }

        public async Task<ChangePasswordResponse> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(
                    predicate: u => u.Id == command.UserId,
                    cancellationToken: cancellationToken
                );

            _authBusinessRules.UserShouldBeExistsWhenSelected(user);
            _authBusinessRules.UserPasswordShouldBeMatch(user!, command.Request.OldPassword);

            HashingHelper.CreatePasswordHash(command.Request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
            user!.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _userRepository.UpdateAsync(user);

            return new ChangePasswordResponse
            {
                Message = AuthMessages.PasswordChangedSuccessfully
            };
        }
    }
}
