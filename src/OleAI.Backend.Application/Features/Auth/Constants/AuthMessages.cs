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
        public const string LoginInformationNotCorrect = "E-posta veya şifre hatalı.";
        public const string AccountRecoverySubject = "OleAI - Şifre Sıfırlama Kodu";
        public const string SecurityKeyUpdateSuccess = "Şifreniz başarıyla değiştirildi.";
        public const string AccountRecoverySuccess = "Şifreniz başarıyla sıfırlandı. Yeni şifrenizle giriş yapabilirsiniz.";
        public const string RecoveryEmailSentMessage = "Eğer sistemimizde böyle bir kayıt varsa, şifre sıfırlama kodu e-posta adresinize gönderilmiştir.";
        public const string InvalidRefreshToken = "Oturumunuzun süresi dolmuş veya geçersiz. Lütfen tekrar giriş yapın.";
        public const string ResendOtpCodeSuccessfully = "Yeni doğrulama kodu e-posta adresinize gönderildi.";
    }
}
