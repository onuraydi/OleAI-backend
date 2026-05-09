using System.Security.Claims;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Features.Users.Dtos.Responses;
using OleAI.Backend.Application.Features.Users.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;

namespace OleAI.Backend.Application.Features.Users.Queries.GetUserProfile
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICacheService _cacheService;
        private readonly UserBusinessRules _userBusinessRules;

        private const string ProfileCacheKeyPrefix = "user-profile:";

        public GetUserProfileQueryHandler(
            IUserRepository userRepository,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ICacheService cacheService,
            UserBusinessRules userBusinessRules)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _cacheService = cacheService;
            _userBusinessRules = userBusinessRules;
        }

        public async Task<UserProfileResponse> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(
                _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var cacheKey = $"{ProfileCacheKeyPrefix}{userId}";

            var cachedProfile = await _cacheService.GetAsync<UserProfileResponse>(cacheKey);
            if (cachedProfile is not null)
                return cachedProfile;

            var user = await _userRepository.GetAsync(
                predicate: u => u.Id == userId,
                include: q => q
                    .Include(u => u.UserDietaryPreferences)
                        .ThenInclude(udp => udp.DietaryPreference)
                    .Include(u => u.UserAllergies)
                        .ThenInclude(ua => ua.AllergyRestriction)
                    .Include(u => u.UserDislikedIngredients)
                        .ThenInclude(udi => udi.Ingredient),
                enableTracking: false,
                cancellationToken: cancellationToken);

            _userBusinessRules.CheckIfUserExists(user);

            var response = _mapper.Map<UserProfileResponse>(user!);

            await _cacheService.SetAsync(cacheKey, response, absoluteExpiration: TimeSpan.FromMinutes(30));

            return response;
        }
    }
}
