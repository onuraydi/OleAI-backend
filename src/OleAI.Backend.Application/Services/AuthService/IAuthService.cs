using System;
using System.Threading;
using System.Threading.Tasks;

namespace OleAI.Backend.Application.Services.AuthService
{
    public interface IAuthService
    {
        Task DeleteOldRefreshTokensAsync(Guid userId, CancellationToken cancellationToken);
    }
}
