using System.Threading.Tasks;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Core.Security.Hashing;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Features.Auth.Rules
{
    public class AuthBusinessRules
    {
        private readonly IUserRepository _userRepository;

        public AuthBusinessRules(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task EmailCannotBeDuplicatedWhenRegistered(string email)
        {
            User? user = await _userRepository.GetAsync(predicate: u => u.Email == email);

            if (user != null)
            {
                throw new BusinessException(AuthMessages.UserMailAlreadyExists);
            }
        }

        public void UserShouldBeExistsWhenSelected(User? user)
        {
            if (user == null)
                throw new BusinessException(AuthMessages.UserNotFound);
        }

        public async Task UserAlreadyVerified(string email)
        {
            User? User = await _userRepository.GetAsync(predicate: u => u.Email == email);

            if (User!.Status)
            {
                throw new BusinessException(AuthMessages.UserAlreadyVerified);
            }
        }

        public void VerificationCodeShouldBeValid(string cachedCode, string otpCode)
        {
            if (string.IsNullOrEmpty(cachedCode) || cachedCode != otpCode)
                throw new BusinessException(AuthMessages.VerificationCodeIsIncorrectOrExpired);
        }

        public void UserShouldBeVerifiedEmail(User? user)
        {
            if (!user!.Status)
            {
                throw new BusinessException(AuthMessages.UserShouldBeVerifiedEmail);
            }
        }

        public void PasswordAndEmailShouldBeValid(string password, byte[] userPasswordHash, byte[] userPasswordSalt)
        {
            if (!HashingHelper.VerifyPasswordHash(password, userPasswordHash, userPasswordSalt))
            {
                throw new BusinessException(AuthMessages.PasswordOrEmailNotCorrect);
            }
        }
    }
}
