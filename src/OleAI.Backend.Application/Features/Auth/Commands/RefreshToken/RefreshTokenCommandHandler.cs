using System.Security.Claims;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Auth.Dtos.Responses;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Services.AuthService;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Helpers;

namespace OleAI.Backend.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITokenHelper _tokenHelper;
        private readonly AuthBusinessRules _authBusinessRules;
        private readonly IAuthService _authService;

        public RefreshTokenCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            IUserRepository userRepository,
            ITokenHelper tokenHelper,
            AuthBusinessRules authBusinessRules,
            IAuthService authService)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userRepository = userRepository;
            _tokenHelper = tokenHelper;
            _authBusinessRules = authBusinessRules;
            _authService = authService;
        }

        public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            var refreshToken = await _refreshTokenRepository.GetAsync(
                predicate: rt => rt.Token == command.Request.Token,
                cancellationToken: cancellationToken
            );

            _authBusinessRules.RefreshTokenShouldBeActive(refreshToken);

            var user = await _userRepository.GetAsync(
                predicate: u => u.Id == refreshToken!.UserId,
                include: m => m.Include(c => c.UserOperationClaims).ThenInclude(uoc => uoc.operationClaim),
                cancellationToken: cancellationToken
            );

            _authBusinessRules.UserShouldBeExistsWhenSelected(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user!.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            };
            foreach (var userRole in user.UserOperationClaims)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.operationClaim.Name));
            }

            var newAccessToken = _tokenHelper.CreateToken(claims);
            var newCoreRefreshToken = _tokenHelper.CreateRefreshToken();

            refreshToken!.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = command.Request.IpAddress;
            refreshToken.ReplacedByToken = newCoreRefreshToken.Token;
            refreshToken.ReasonRevoked = "Replaced by new token";

            await _refreshTokenRepository.UpdateAsync(refreshToken);

            var newDomainRefreshToken = new Domain.Entities.RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = newCoreRefreshToken.Token,
                Expires = newCoreRefreshToken.Expiration,
                CreatedDate = DateTime.UtcNow,
                CreatedByIp = command.Request.IpAddress
            };

            await _authService.DeleteOldRefreshTokensAsync(user.Id, cancellationToken);
            await _refreshTokenRepository.AddAsync(newDomainRefreshToken);

            return new RefreshTokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newCoreRefreshToken
            };
        }
    }
}
