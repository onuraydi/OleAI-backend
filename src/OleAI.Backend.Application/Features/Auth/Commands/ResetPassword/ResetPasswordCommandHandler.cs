using MediatR;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Features.Auth.Dtos.Responses;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.Security.Hashing;

namespace OleAI.Backend.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICacheService _cacheService;
        private readonly AuthBusinessRules _authBusinessRules;

        public ResetPasswordCommandHandler(IUserRepository userRepository, ICacheService cacheService, AuthBusinessRules authBusinessRules)
        {
            _userRepository = userRepository;
            _cacheService = cacheService;
            _authBusinessRules = authBusinessRules;
        }

        public async Task<ResetPasswordResponse> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(
                    predicate: u => u.Email == command.Request.Email,
                    cancellationToken: cancellationToken
                );

            _authBusinessRules.UserShouldBeExistsWhenSelected(user);

            string redisKey = $"OTP:Reset{command.Request.Email}";
            var cachedCode = await _cacheService.GetAsync<string>(redisKey);

            _authBusinessRules.VerificationCodeShouldBeValid(cachedCode, command.Request.OtpCode);

            HashingHelper.CreatePasswordHash(command.Request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
            user!.PasswordHash = passwordHash;
            user!.PasswordSalt = passwordSalt;

            await _userRepository.UpdateAsync(user);
            await _cacheService.RemoveAsync(redisKey);

            return new ResetPasswordResponse
            {
                Message = AuthMessages.PasswordResetSuccessfully
            };
        }
    }
}
