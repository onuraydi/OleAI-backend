using System.Security.Claims;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Features.Auth.Dtos.Responses;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Services.AuthService;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Core.CrossCuttingConcerns.Mailing;
using OleAI.Backend.Core.Helpers;

namespace OleAI.Backend.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoggedResponse>
    {
        private static readonly (int Attempts, TimeSpan? LockoutDuration, string Subject, string Body)[] LockoutPolicies =
        {
            (25, null, AuthMessages.LockoutPermanentSubject, AuthMessages.LockoutPermanentBody),
            (20, TimeSpan.FromHours(24), AuthMessages.Lockout24HoursSubject, AuthMessages.Lockout24HoursBody),
            (15, TimeSpan.FromHours(1), AuthMessages.Lockout1HourSubject, AuthMessages.Lockout1HourBody),
            (10, TimeSpan.FromMinutes(15), AuthMessages.Lockout15MinutesSubject, AuthMessages.Lockout15MinutesBody),
            (5, TimeSpan.FromMinutes(5), AuthMessages.Lockout5MinutesSubject, AuthMessages.Lockout5MinutesBody)
        };

        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly AuthBusinessRules _authBusinessRules;
        private readonly ITokenHelper _tokenHelper;
        private readonly IAuthService _authService;
        private readonly IMailService _mailService;

        public LoginCommandHandler(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            AuthBusinessRules authBusinessRules,
            ITokenHelper tokenHelper,
            IAuthService authService,
            IMailService mailService)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _authBusinessRules = authBusinessRules;
            _tokenHelper = tokenHelper;
            _authService = authService;
            _mailService = mailService;
        }

        public async Task<LoggedResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(
                predicate: u => u.Email == command.Request.Email,
                include: m => m.Include(c => c.UserOperationClaims).ThenInclude(uoc => uoc.operationClaim),
                cancellationToken: cancellationToken
                );

            var isValid = _authBusinessRules.ValidateCredentials(user, command.Request.Password);

            if (user != null)
            {
                if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
                {
                    throw new BusinessException($"Hesabınız çok fazla hatalı giriş denemesi sebebiyle kilitlenmiştir. {user.LockoutEnd.Value.ToLocalTime():HH:mm} tarihinden sonra tekrar deneyiniz.");
                }

                if (!user.IsActive)
                {
                    throw new BusinessException(AuthMessages.AccountNotActiveMessage);
                }

                if (!isValid)
                {
                    user.FailedLoginAttempts++;

                    var policy = LockoutPolicies.FirstOrDefault(p => user.FailedLoginAttempts == p.Attempts);
                    if (policy != default)
                    {
                        if (policy.LockoutDuration == null)
                        {
                            user.IsActive = false;
                        }
                        else
                        {
                            user.LockoutEnd = DateTime.UtcNow.Add(policy.LockoutDuration.Value);
                        }
                        await _mailService.sendAsync(user.Email, policy.Subject, policy.Body);
                    }

                    await _userRepository.UpdateAsync(user);
                    throw new BusinessException(AuthMessages.InvalidCredentials);
                }

                if (user.FailedLoginAttempts > 0 || user.LockoutEnd.HasValue)
                {
                    user.FailedLoginAttempts = 0;
                    user.LockoutEnd = null;
                    await _userRepository.UpdateAsync(user);
                }
            }
            else
            {
                if (!isValid) throw new BusinessException(AuthMessages.InvalidCredentials);
            }

            var verifiedUser = user!;

            _authBusinessRules.UserShouldBeVerifiedEmail(verifiedUser);
            // user.IsActive kontrolünü yukarıda önceden yaptık.

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, verifiedUser.Id.ToString()),
                new Claim(ClaimTypes.Email, verifiedUser.Email),
                new Claim(ClaimTypes.Name, $"{verifiedUser.FirstName} {verifiedUser.LastName}")
            };

            foreach (var userRole in verifiedUser.UserOperationClaims)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.operationClaim.Name));
            }

            var accessToken = _tokenHelper.CreateToken(claims);
            var coreRefreshToken = _tokenHelper.CreateRefreshToken();

            var domainRefreshToken = new Domain.Entities.RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = verifiedUser.Id,
                Token = coreRefreshToken.Token,
                Expires = coreRefreshToken.Expiration,
                CreatedDate = DateTime.UtcNow,
                CreatedByIp = command.IpAddress
            };

            await _authService.DeleteOldRefreshTokensAsync(verifiedUser.Id, cancellationToken);
            await _refreshTokenRepository.AddAsync(domainRefreshToken);

            return new LoggedResponse
            {
                AccessToken = accessToken,
                RefreshToken = coreRefreshToken
            };
        }
    }
}
