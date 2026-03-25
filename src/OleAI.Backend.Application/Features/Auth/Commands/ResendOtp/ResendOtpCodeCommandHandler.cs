using MediatR;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Features.Auth.Dtos.Responses;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.CrossCuttingConcerns.Mailing;
using OleAI.Backend.Core.Security.OTP;

namespace OleAI.Backend.Application.Features.Auth.Commands.ResendOtp
{
    public class ResendOtpCodeCommandHandler : IRequestHandler<ResendOtpCommand, ResendedOtpCodeResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthBusinessRules _authBusinessRules;
        private readonly ICacheService _cacheService;
        private readonly IMailService _mailService;

        public ResendOtpCodeCommandHandler(IUserRepository userRepository, AuthBusinessRules authBusinessRules, ICacheService cacheService, IMailService mailService)
        {
            _userRepository = userRepository;
            _authBusinessRules = authBusinessRules;
            _cacheService = cacheService;
            _mailService = mailService;
        }

        public async Task<ResendedOtpCodeResponse> Handle(ResendOtpCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(u => u.Email == command.Request.Email, cancellationToken: cancellationToken);
            _authBusinessRules.UserShouldBeExistsWhenSelected(user);
            await _authBusinessRules.UserAlreadyVerified(user!.Email);

            string otpCode = OtpHelper.GenerateSixDigitCode();
            string redisKey = $"OTP:Verify:{user.Email}";

            await _cacheService.SetAsync(redisKey, otpCode, absoluteExpiration: TimeSpan.FromMinutes(3));

            string mailBody = $"<h2>OleAI Doğrulama Kodu</h2><p>Yeni kodunuz:</p><h1 style='color:#4CAF50;'>{otpCode}</h1>";
            await _mailService.sendAsync(user.Email, AuthMessages.EmailVerificationSubject, mailBody);

            return new ResendedOtpCodeResponse
            {
                Message = AuthMessages.ResendOtpCodeSuccessfully
            };
        }
    }
}
