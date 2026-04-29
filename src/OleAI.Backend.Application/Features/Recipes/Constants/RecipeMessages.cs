namespace OleAI.Backend.Application.Features.Recipes.Constants
{
    public static class RecipeMessages
    {
        public const string InvalidIngredientsSelectedForRecipe = "Tarife eklenmek istenen bazı malzemeler sistemde bulunamadı. Lütfen geçerli malzemeler seçin.";
        public const string UnauthorizedAccess = "Yetkisiz işlem: Kullanıcı kimliği doğrulanamadı.";
        public const string RecipeAddedSuccessfully = "Tarif başarıyla eklendi.";
        public const string AdminRoleRequired = "Bu işlem için yönetici yetkisine sahip olmanız gerekmektedir.";
        public const string AiModeratorDetectedHarmfulContent = "Yapay zeka moderatörü tarafından anlamsız, spam veya zararlı içerik tespit edildi.";
        public const string RecipeRejectedDueToSystemStandards = "Tarifiniz sistem standartlarına uymadığı için reddedildi.";
        public const string RecipeCreatedAndSavedAsPrivate = "Tarif başarıyla oluşturuldu ve sadece sizin görebileceğiniz şekilde kaydedildi.";
        public const string RecipeCreatedAndPendingAdminApproval = "Tarifiniz oluşturuldu ve yönetici onayı için sıraya alındı.";
        public const string RecipeApprovedAndPublished = "Tarif başarıyla onaylandı ve topluluğa açıldı.";
        public const string RecipeNotFoundOrDeleted = "İşlem yapmak istediğiniz tarif bulunamadı veya sistemden silinmiş.";
        public const string UnauthorizedRecipeAccess = "Bu tarif üzerinde değişiklik yapma veya silme yetkiniz bulunmuyor.";
    }
}
