using AutoMapper;
using MediatR;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Features.Auth.Dtos.Responses;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.CrossCuttingConcerns.Mailing;
using OleAI.Backend.Core.Security.Hashing;
using OleAI.Backend.Core.Security.OTP;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisteredResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthBusinessRules _authBusinessRules;
        private readonly ICacheService _cacheService;
        private readonly IMailService _mailService;
        private readonly IMapper _mapper;

        public RegisterCommandHandler(IUserRepository userRepository, AuthBusinessRules authBusinessRules, ICacheService cacheService, IMailService mailService, IMapper mapper)
        {
            _userRepository = userRepository;
            _authBusinessRules = authBusinessRules;
            _cacheService = cacheService;
            _mailService = mailService;
            _mapper = mapper;
        }

        public async Task<RegisteredResponse> Handle(RegisterCommand command, CancellationToken cancellationToken)
        {
            await _authBusinessRules.EmailCannotBeDuplicatedWhenRegistered(command.Request.Email);

            HashingHelper.CreatePasswordHash(command.Request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            User newUser = _mapper.Map<User>(command.Request);
            newUser.Id = Guid.NewGuid();
            newUser.PasswordHash = passwordHash;
            newUser.PasswordSalt = passwordSalt;
            newUser.Status = false;

            await _userRepository.AddAsync(newUser);

            string otpCode = OtpHelper.GenerateSixDigitCode();
            await _cacheService.SetAsync(
                key: $"OTP:Verify:{newUser.Email}",
                value: otpCode,
                absoluteExpiration: TimeSpan.FromMinutes(3)
            );

            string mailBody = $@"<h2>OleAI'a Hoş Geldiniz!</h2><h1 style='color: #4CAF50;'>{otpCode}</h1>";
            await _mailService.sendAsync(newUser.Email, AuthMessages.EmailVerificationSubject, mailBody);

            RegisteredResponse response = _mapper.Map<RegisteredResponse>(newUser);
            response.Message = AuthMessages.SuccessfulRegistration;

            return response;
        }
    }
}
