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
        public const string SpamDetectedAfterUpdate = "Güncelleme sonrası spam içerik tespit edildi.";
        public const string RecipeUpdatedButRejectedDueToSpam = "Tarif güncellendi ancak spam içerik nedeniyle reddedildi.";
        public const string RecipeUpdatedSuccessfully = "Tarif başarıyla güncellendi.";
        public const string RecipeDeletedWithRelatedContent = "Tarif ve ilgili tüm içerikler başarıyla silindi.";
        public const string VisibilityStatusIsAlreadySame = "Tarif zaten bu görünürlük durumunda.";
        public const string RecipePublishingRejectedDueToContent = "Tarif topluluğa açılırken spam içerik tespit edildi ve reddedildi.";
        public const string VisibilityUpdateFailedStandards = "Tarif görünürlüğü değiştirilirken sistem standartlarına uymadığı için reddedildi.";
        public const string RecipePublishedAndQueuedForReview = "Tarif topluluğa açıldı ve yönetici onayı için sıraya alındı.";
        public const string RecipeVisibilityChangedToPrivate = "Tarif gizliye (Private) çekildi, artık sadece siz görebilirsiniz.";
        public const string RecipeNotFoundMessage = "Tarif bulunamadı.";
        public const string EnjoyAndInventoryUpdatedWithShoppingList = "Afiyet olsun! Malzemeler dolabınızdan düşüldü ve eksilen ürünler alışveriş listenize eklendi.";
        public const string EnjoyAndIngredientsRemovedFromInventory = "Afiyet olsun! Malzemeler dolabınızdan başarıyla düşüldü.";
        public const string RecipeNotOpenForInteractionYet = "Bu tarif henüz etkileşime açık değil.";
        public const string RecipeNotFound = "Tarif bulunamadı.";
        public const string TargetCommentMissing = "Cevap vermeye çalıştığınız yorum bulunamadı veya silinmiş.";
        public const string UnauthorizedCommentAction = "Bu yorum üzerinde işlem yapma yetkiniz yok.";
        public const string CommentUpdatedSuccessfullyMessage = "Yorumunuz başarıyla güncellendi.";
        public const string CommentDeletedSuccessfullyMessage = "Yorumunuz başarıyla silindi.";
        public const string RecipeRestored = "Tarif başarıyla çöp kutusundan geri yüklendi.";
        public const string RecipeHardDeleted = "Tarif sistemden kalıcı olarak silindi.";
    }
}
