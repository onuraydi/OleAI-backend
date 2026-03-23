using System.Security.Cryptography;

namespace OleAI.Backend.Core.Security.OTP
{
    public static class OtpHelper
    {
        public static string GenerateSixDigitCode()
        {
            int randomNumber = RandomNumberGenerator.GetInt32(100000, 999999);

            return randomNumber.ToString();
        }
    }
}
