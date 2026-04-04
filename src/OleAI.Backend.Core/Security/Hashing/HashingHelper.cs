using System.Security.Cryptography;
using System.Text;

namespace OleAI.Backend.Core.Security.Hashing
{
    public static class HashingHelper
    {
        // OWASP 2024 standartlarına göre PBKDF2-HMAC-SHA512 için önerilen iterasyon sayısı.
        // Bu değer brute-force saldırılarını GPU üzerinde bile inanılmaz derecede maliyetli hale getirir.
        private const int Pbkdf2IterCount = 350000;
        private const int Pbkdf2SubkeyLength = 64; // 512 bits (SHA512 boyutuna eşdeğer)
        private const int SaltSize = 16; // 128 bits

        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            passwordSalt = RandomNumberGenerator.GetBytes(SaltSize);

            passwordHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                passwordSalt,
                Pbkdf2IterCount,
                HashAlgorithmName.SHA512,
                Pbkdf2SubkeyLength);
        }

        public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            if (passwordSalt.Length == 128)
            {
                using var hmac = new HMACSHA512(passwordSalt);
                var computedHashLegacy = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return CryptographicOperations.FixedTimeEquals(computedHashLegacy, passwordHash);
            }

            var computedHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                passwordSalt,
                Pbkdf2IterCount,
                HashAlgorithmName.SHA512,
                passwordHash.Length);

            return CryptographicOperations.FixedTimeEquals(computedHash, passwordHash);
        }
    }
}
