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

        private const string ProfileCacheKeyPrefix = "user-profile:";

        public UpdateUserProfileCommandHandler(
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,
            ICacheService cacheService,
            UserBusinessRules userBusinessRules)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _cacheService = cacheService;
            _userBusinessRules = userBusinessRules;
        }

        public async Task<string> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(
                _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var user = await _userRepository.GetAsync(
                predicate: u => u.Id == userId,
                cancellationToken: cancellationToken);

            _userBusinessRules.CheckIfUserExists(user);

            user!.FirstName = request.FirstName.Trim();
            user.LastName = request.LastName.Trim();

            await _userRepository.UpdateAsync(user, cancellationToken);

            // Cache Invalidation — eski profil verisi artık geçersiz
            await _cacheService.RemoveAsync($"{ProfileCacheKeyPrefix}{userId}");

            return UserMessages.ProfileUpdatedSuccessfully;
        }
    }
}
