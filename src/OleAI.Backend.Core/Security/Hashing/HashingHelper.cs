using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OleAI.Backend.Core.Security.Hashing
{
    public static class HashingHelper
    {
        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            // SENIOR CHECK: Standart bir '==' veya 'SequenceEqual' kullanmak yerine 
            // FixedTimeEquals kullanıyoruz. Bu işlem, hackerların süre farklarını ölçerek
            // şifreyi tahmin etmeye çalıştığı 'Timing Attack' (Zamanlama Saldırısı) türlerini tamamen engeller.
            return CryptographicOperations.FixedTimeEquals(computedHash, passwordHash);
        }
    }
}
