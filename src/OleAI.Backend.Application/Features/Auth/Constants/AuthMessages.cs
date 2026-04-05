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
        public const string TooManyOtpAttempts = "Çok fazla hatalı giriş yaptınız. Güvenliğiniz için doğrulama kodunuz iptal edildi. Lütfen yeni bir kod talep edin.";
        public const string UserVerifiedMessage = "E-posta adresiniz başarıyla doğrulandı. Artık giriş yapabilirsiniz.";
        public const string UserShouldBeVerifiedEmail = "Lütfen önce e-posta adresinizi doğrulayın.";
        public const string LoginInformationNotCorrect = "E-posta adresi veya şifre hatalı.";
        public const string InvalidCredentials = "E-posta adresi veya şifre hatalı.";
        public const string AccountRecoverySubject = "OleAI - Şifre Sıfırlama Kodu";
        public const string SecurityKeyUpdateSuccess = "Şifreniz başarıyla değiştirildi.";
        public const string AccountRecoverySuccess = "Şifreniz başarıyla sıfırlandı. Yeni şifrenizle giriş yapabilirsiniz.";
        public const string RecoveryEmailSentMessage = "Eğer sistemimizde böyle bir kayıt varsa, şifre sıfırlama kodu e-posta adresinize gönderilmiştir.";
        public const string InvalidRefreshToken = "Oturumunuzun süresi dolmuş veya geçersiz. Lütfen tekrar giriş yapın.";
        public const string ResendOtpCodeSuccessfully = "Yeni doğrulama kodu e-posta adresinize gönderildi.";
        public const string LogoutSuccesfullyMessage = "Başarıyla çıkış yapıldı.";
        public const string LogoutSuccesfullyWithUserRequestMessage = "Kullanıcı isteğiyle çıkış yapıldı (Logout).";
        public const string AccountNotActiveMessage = "Kullanıcı hesabı aktif değil veya askıya alınmış.";
        public const string TargetUserNotFound = "Belirtilen kullanıcı bulunamadı.";
        public const string OperationClaimNotFound = "Belirtilen yetki (rol) bulunamadı.";
        public const string UserAlreadyHasRole = "Kullanıcı zaten bu yetkiye sahip.";
        public const string RoleAssignedSuccessfully = "Kullanıcıya ilgili rol başarıyla atandı.";

        // Brute-force / Lockout Messages
        public const string LockoutPermanentSubject = "OleAI - Hesabınız Güvenlik Nedeniyle Kapatıldı";
        public const string LockoutPermanentBody = "Hesabınıza 25 kez peş peşe hatalı giriş yapıldı. Güvenliğiniz için hesabınız pasif duruma çekildi. Lütfen yöneticinizle iletişime geçin.";

        public const string Lockout24HoursSubject = "OleAI - Şüpheli Giriş Uyarıları (24 Saat Kilit)";
        public const string Lockout24HoursBody = "Hesabınıza peş peşe hatalı giriş denemeleri yapıldığı için hesabınız 24 saat kilitlendi. Bu işlemi siz yapmadıysanız şifrenizi hemen sıfırlayın.";

        public const string Lockout1HourSubject = "OleAI - Şüpheli Giriş Uyarıları (1 Saat Kilit)";
        public const string Lockout1HourBody = "Hesabınıza art arda hatalı denemeler yapıldı ve hesabınız 1 saat kilitlendi.";

        public const string Lockout15MinutesSubject = "OleAI - Şüpheli Giriş Uyarıları (15 Dakika Kilit)";
        public const string Lockout15MinutesBody = "10 kez hatalı giriş denemesi sebebiyle 15 dakika kilitlendiniz.";

        public const string Lockout5MinutesSubject = "OleAI - Şüpheli Aktivite Tespit Edildi";
        public const string Lockout5MinutesBody = "5 kez hatalı şifre girdiniz. Sistem sizi 5 dakika oyalıyor. Şifrenizi hatırlamıyorsanız şifremi unuttum adımını kullanınız.";
    }
}
