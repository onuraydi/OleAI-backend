using System.Security.Cryptography;
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

        private static readonly byte[] DummyPasswordHash;
        private static readonly byte[] DummyPasswordSalt;

        static AuthBusinessRules()
        {
            HashingHelper.CreatePasswordHash(
                Guid.NewGuid().ToString(),
                out byte[] hash,
                out byte[] salt);
            DummyPasswordHash = hash;
            DummyPasswordSalt = salt;
        }

        public AuthBusinessRules(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Validates user credentials in a single operation, preventing User Enumeration
        /// and Timing Attack vulnerabilities.
        ///
        /// Security guarantees:
        /// 1. Returns a single generic error message for both "user not found" and "wrong password".
        /// 2. Runs PBKDF2 regardless of whether the user exists, so response time doesn't leak info.
        /// </summary>
        public bool ValidateCredentials(User? user, string password)
        {
            byte[] hashToVerify = user?.PasswordHash ?? DummyPasswordHash;
            byte[] saltToVerify = user?.PasswordSalt ?? DummyPasswordSalt;

            bool isPasswordValid = HashingHelper.VerifyPasswordHash(password, hashToVerify, saltToVerify);

            return user != null && isPasswordValid;
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
            User? user = await _userRepository.GetAsync(predicate: u => u.Email == email);

            if (user != null && user.IsEmailVerified)
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
            if (user != null && !user.IsEmailVerified)
            {
                throw new BusinessException(AuthMessages.UserShouldBeVerifiedEmail);
            }
        }

        public void UserShouldBeActive(User? user)
        {
            if (user != null && !user.IsActive)
            {
                throw new BusinessException(AuthMessages.AccountNotActiveMessage);
            }
        }

        public void PasswordAndEmailShouldBeValid(string password, byte[] userPasswordHash, byte[] userPasswordSalt)
        {
            if (!HashingHelper.VerifyPasswordHash(password, userPasswordHash, userPasswordSalt))
            {
                throw new BusinessException(AuthMessages.LoginInformationNotCorrect);
            }
        }

        public void UserPasswordShouldBeMatch(User user, string password)
        {
            if (!HashingHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                throw new BusinessException("Mevcut şifreniz hatalı.");
        }

        public void RefreshTokenShouldBeActive(RefreshToken? refreshToken)
        {
            if (refreshToken == null || !refreshToken.IsActive)
                throw new AuthenticationException(AuthMessages.InvalidRefreshToken);
        }
    }
}
