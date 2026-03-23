namespace OleAI.Backend.Application.Features.Auth.Constants
{
    public static class AuthMessages
    {
        public const string UserMailAlreadyExists = "Bu e-posta adresi zaten kullanımda.";
        public const string UserNotFound = "Kullanıcı bulunamadı veya e-posta/şifre hatalı.";
        public const string SuccessfulRegistration = "Kayıt başarılı. Lütfen e-postanıza gönderilen 6 haneli kodu girerek hesabınızı doğrulayın.";
        public const string EmailVerificationSubject = "OleAI - E-posta Doğrulama Kodu";
        public const string UserAlreadyVerified = "Bu hesap zaten doğrulanmış.";
        public const string VerificationCodeIsIncorrectOrExpired = "Doğrulama kodu hatalı veya süresi dolmuş.";
        public const string UserVerifiedMessage = "E-posta adresiniz başarıyla doğrulandı. Artık giriş yapabilirsiniz.";
        public const string UserShouldBeVerifiedEmail = "Lütfen önce e-posta adresinizi doğrulayın.";
        public const string PasswordOrEmailNotCorrect = "E-posta veya şifre hatalı.";
    }
}
