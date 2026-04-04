using MediatR;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Features.Auth.Dtos.Responses;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, LoggedOutResponse>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<LoggedOutResponse> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return new LoggedOutResponse { Message = AuthMessages.LogoutSuccesfullyMessage };
            }

            var refreshToken = await _refreshTokenRepository.GetAsync(
                predicate: rt => rt.Token == request.RefreshToken,
                cancellationToken: cancellationToken
            );

            if (refreshToken != null && refreshToken.IsActive)
            {
                refreshToken.Revoked = DateTime.UtcNow;
                refreshToken.RevokedByIp = request.IpAddress;
                refreshToken.ReasonRevoked = AuthMessages.LogoutSuccesfullyWithUserRequestMessage;

                await _refreshTokenRepository.UpdateAsync(refreshToken);
            }

            return new LoggedOutResponse { Message = AuthMessages.LogoutSuccesfullyMessage };
        }
    }
}
