using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Notifications.Commands.RegisterDevice
{
    public class RegisterDeviceCommand : IRequest<bool>
    {
        public string DeviceToken { get; set; } = null!;
        public DevicePlatform Platform { get; set; }
    }

    public class RegisterDeviceCommandHandler : IRequestHandler<RegisterDeviceCommand, bool>
    {
        private readonly IUserDeviceRepository _userDeviceRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RegisterDeviceCommandHandler(IUserDeviceRepository userDeviceRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userDeviceRepository = userDeviceRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Handle(RegisterDeviceCommand request, CancellationToken cancellationToken)
        {
            var userIdString = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                throw new UnauthorizedAccessException("Kullanıcı kimliği doğrulanamadı.");
            }

            var existingDevice = await _userDeviceRepository.GetAsync(
                predicate: d => d.DeviceToken == request.DeviceToken && d.UserId == userId,
                cancellationToken: cancellationToken);

            if (existingDevice != null)
            {
                // Already registered
                return true;
            }

            var newDevice = new UserDevice(Guid.NewGuid(), userId, request.DeviceToken, request.Platform);
            await _userDeviceRepository.AddAsync(newDevice);

            return true;
        }
    }
}
