using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using OleAI.Backend.Application.Features.Users.Constants;
using OleAI.Backend.Application.Features.Users.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;

namespace OleAI.Backend.Application.Features.Users.Commands.UpdateUserProfile
{
    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICacheService _cacheService;
        private readonly UserBusinessRules _userBusinessRules;
        private readonly OleAI.Backend.Application.Services.ImageStorage.IImageService _imageService;

        private const string ProfileCacheKeyPrefix = "user-profile:";

        public UpdateUserProfileCommandHandler(
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,
            ICacheService cacheService,
            UserBusinessRules userBusinessRules,
            OleAI.Backend.Application.Services.ImageStorage.IImageService imageService)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _cacheService = cacheService;
            _userBusinessRules = userBusinessRules;
            _imageService = imageService;
        }

        public async Task<string> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(
                _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var user = await _userRepository.GetAsync(
                predicate: u => u.Id == userId,
                cancellationToken: cancellationToken);

            _userBusinessRules.CheckIfUserExists(user);

            if (!string.IsNullOrEmpty(request.FirstName))
                user!.FirstName = request.FirstName.Trim();

            if (!string.IsNullOrEmpty(request.LastName))
                user.LastName = request.LastName.Trim();

            if (!string.IsNullOrEmpty(request.Username))
                user.Username = request.Username.Trim();

            if (request.BirthDate.HasValue)
                user.BirthDate = request.BirthDate.Value;

            user.IsOnboarded = request.IsOnboarded;

            if (request.ProfileImage != null)
            {
                var uploadResult = await _imageService.UploadAsync(request.ProfileImage, "profile_images", cancellationToken);
                user.ProfileImageUrl = uploadResult.Url;
            }

            await _userRepository.UpdateAsync(user, cancellationToken);

            // Cache Invalidation — eski profil verisi artık geçersiz
            await _cacheService.RemoveAsync($"{ProfileCacheKeyPrefix}{userId}");

            return UserMessages.ProfileUpdatedSuccessfully;
        }
    }
}
