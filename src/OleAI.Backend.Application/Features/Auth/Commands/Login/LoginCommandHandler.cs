using System.Security.Claims;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Auth.Dtos.Responses;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Helpers;

namespace OleAI.Backend.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoggedResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly AuthBusinessRules _authBusinessRules;
        private readonly ITokenHelper _tokenHelper;

        public LoginCommandHandler(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, AuthBusinessRules authBusinessRules, ITokenHelper tokenHelper)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _authBusinessRules = authBusinessRules;
            _tokenHelper = tokenHelper;
        }

        public async Task<LoggedResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(
                predicate: u => u.Email == command.Request.Email,
                include: m => m.Include(c => c.UserOperationClaims).ThenInclude(uoc => uoc.operationClaim),
                cancellationToken: cancellationToken
                );

            _authBusinessRules.UserShouldBeExistsWhenSelected(user);
            _authBusinessRules.UserShouldBeVerifiedEmail(user);
            _authBusinessRules.PasswordAndEmailShouldBeValid(command.Request.Password, user.PasswordHash, user.PasswordSalt);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            };

            foreach (var userRole in user.UserOperationClaims)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.operationClaim.Name));
            }

            var accessToken = _tokenHelper.CreateToken(claims);
            var coreRefreshToken = _tokenHelper.CreateRefreshToken();

            var domainRefreshToken = new Domain.Entities.RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = coreRefreshToken.Token,
                Expires = coreRefreshToken.Expiration,
                CreatedDate = DateTime.UtcNow,
                CreatedByIp = command.IpAddress
            };

            await _refreshTokenRepository.AddAsync(domainRefreshToken);

            return new LoggedResponse
            {
                AccessToken = accessToken,
                RefreshToken = coreRefreshToken
            };
        }
    }
}
