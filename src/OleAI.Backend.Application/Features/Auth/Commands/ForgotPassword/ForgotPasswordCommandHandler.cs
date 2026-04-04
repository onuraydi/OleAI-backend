using MediatR;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Features.Auth.Dtos.Responses;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.CrossCuttingConcerns.Mailing;
using OleAI.Backend.Core.Security.OTP;

namespace OleAI.Backend.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICacheService _cacheService;
        private readonly IMailService _mailService;
        private readonly AuthBusinessRules _authBusinessRules;

        public ForgotPasswordCommandHandler(IUserRepository userRepository, ICacheService cacheService, IMailService mailService, AuthBusinessRules authBusinessRules)
        {
            _userRepository = userRepository;
            _cacheService = cacheService;
            _mailService = mailService;
            _authBusinessRules = authBusinessRules;
        }

        public async Task<ForgotPasswordResponse> Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(
                    predicate: u => u.Email == command.Request.Email,
                    cancellationToken: cancellationToken
                );

            if (user != null)
            {
                string otpCode = OtpHelper.GenerateSixDigitCode();
                string redisKey = $"OTP:Reset:{user.Email}";

                await _cacheService.SetAsync(redisKey, otpCode, absoluteExpiration: TimeSpan.FromMinutes(5));

                string mailBody = $"<h2>OleAI Şifre Sıfırlama</h2><p>Şifrenizi sıfırlamak için onay kodunuz:</p><h1 style='color:#FF5722;'>{otpCode}</h1>";
                await _mailService.sendAsync(user.Email, AuthMessages.AccountRecoverySubject, mailBody);
            }

            return new ForgotPasswordResponse
            {
                Message = AuthMessages.RecoveryEmailSentMessage,
            };
        }
    }
}
