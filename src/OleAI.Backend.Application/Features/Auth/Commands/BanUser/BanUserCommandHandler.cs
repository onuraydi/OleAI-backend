using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Features.Auth.Dtos.Responses;
using OleAI.Backend.Application.Services.AuthService;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;

namespace OleAI.Backend.Application.Features.Auth.Commands.BanUser
{
    public class BanUserCommandHandler : IRequestHandler<BanUserCommand, BanUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;

        public BanUserCommandHandler(IUserRepository userRepository, IAuthService authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }

        public async Task<BanUserResponse> Handle(BanUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(u => u.Id == command.Request.UserId, cancellationToken: cancellationToken);

            if (user == null)
            {
                throw new BusinessException(AuthMessages.TargetUserNotFound);
            }

            user.IsActive = false;

            await _userRepository.UpdateAsync(user);

            // Kullanıcıyı sistemden hemen atmak için oturum (refresh token) kayıtlarını siliyoruz.
            await _authService.DeleteOldRefreshTokensAsync(user.Id, cancellationToken);

            return new BanUserResponse
            {
                Message = "Kullanıcı başarıyla banlandı."
            };
        }
    }
}
