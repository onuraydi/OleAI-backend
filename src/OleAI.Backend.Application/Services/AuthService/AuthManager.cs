using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Security.Jwt;

namespace OleAI.Backend.Application.Services.AuthService
{
    public class AuthManager : IAuthService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly TokenOptions _tokenOptions;

        public AuthManager(IRefreshTokenRepository refreshTokenRepository, TokenOptions tokenOptions)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _tokenOptions = tokenOptions;
        }

        public async Task DeleteOldRefreshTokensAsync(Guid userId, CancellationToken cancellationToken)
        {
            // 1. Delete all invalid tokens directly on the DB (Expired or Revoked)
            await _refreshTokenRepository.ExecuteDeleteAsync(
                predicate: rt => rt.UserId == userId && (rt.Expires <= DateTime.UtcNow || rt.Revoked != null),
                cancellationToken: cancellationToken);

            // 2. Fetch the remaining active tokens and enforce device limit
            var maxActiveTokens = _tokenOptions.MaxActiveTokens > 0 ? _tokenOptions.MaxActiveTokens : 3;

            var activeTokensPaging = await _refreshTokenRepository.GetListAsync(
                new QueryOptions<Domain.Entities.RefreshToken>
                {
                    Predicate = rt => rt.UserId == userId && rt.Expires > DateTime.UtcNow && rt.Revoked == null,
                    OrderBy = q => q.OrderByDescending(rt => rt.CreatedDate),
                    Index = 0,
                    Size = 100 // Safely assuming a user will not legitimately exceed 100 active devices in between deletions
                },
                cancellationToken: cancellationToken);

            if (activeTokensPaging.Items.Count >= maxActiveTokens)
            {
                // We keep the first (most recent) `maxActiveTokens - 1` tokens 
                // because we are about to add a NEW token right after calling this method.
                var tokensToDelete = activeTokensPaging.Items
                    .Skip(maxActiveTokens - 1)
                    .ToList();

                if (tokensToDelete.Any())
                {
                    await _refreshTokenRepository.DeleteRangeAsync(
                        entities: tokensToDelete,
                        permanent: true,
                        cancellationToken: cancellationToken);
                }
            }
        }
    }
}
