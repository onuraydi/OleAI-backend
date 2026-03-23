using AutoMapper;
using MediatR;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Features.Auth.Dtos.Responses;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;

namespace OleAI.Backend.Application.Features.Auth.Commands.VerifyEmail
{
    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, verifiedEmailResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICacheService _cacheService;
        private readonly AuthBusinessRules _authBusinessRules;
        private readonly IMapper _mapper;

        public VerifyEmailCommandHandler(IUserRepository userRepository, ICacheService cacheService, AuthBusinessRules authBusinessRules, IMapper mapper)
        {
            _userRepository = userRepository;
            _cacheService = cacheService;
            _authBusinessRules = authBusinessRules;
            _mapper = mapper;
        }

        public async Task<verifiedEmailResponse> Handle(VerifyEmailCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(u => u.Email == command.Request.Email);
            _authBusinessRules.UserShouldBeExistsWhenSelected(user);
            await _authBusinessRules.UserAlreadyVerified(user.Email);

            string redisKey = $"OTP:Verify:{command.Request.Email}";
            var cachedCode = await _cacheService.GetAsync<string>(redisKey);

            _authBusinessRules.VerificationCodeShouldBeValid(cachedCode, command.Request.OtpCode);

            user.Status = true; // TODO: Bu kısım // Entity içindeki bir metot ile durumu güncellemek (Encapsulation) [user.MarkAsEmailVerified();]

            await _userRepository.UpdateAsync(user);
            await _cacheService.RemoveAsync(redisKey);

            verifiedEmailResponse response = _mapper.Map<verifiedEmailResponse>(user);
            response.Message = AuthMessages.UserVerifiedMessage;

            return response;
        }
    }
}
