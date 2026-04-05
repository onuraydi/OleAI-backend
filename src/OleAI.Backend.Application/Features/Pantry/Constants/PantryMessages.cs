namespace OleAI.Backend.Application.Features.Pantry.Constants
{
    public static class PantryMessages
    {
        public const string IngredientNotFoundMessage = "Seçilen malzeme sistemde bulunamadı.";
        public const string ProductSelectionConflictMessage = "Aynı anda hem listeden ürün seçip hem de özel isim giremezsiniz.";
        public const string ProductSelectionRequiredMessage = "Lütfen bir ürün seçin veya adını yazın.";
        public const string IngredientQuantityUpdatedMessage = "Ürün miktarı güncellendi.";
        public const string IngredientAddedPantryMessage = "Ürün dolabınıza eklendi.";
        public const string IngredientSentForAiApprovalMessage = "Ürün talebiniz alındı ve yapay zeka onayına gönderildi. Dolabınızda onay bekliyor olarak görünecektir.";
        public const string RequestNotFoundMessage = "Talep bulunamadı.";
        public const string CategoryAndUnitTypeRequiredMessage = "Yeni ürün eklenirken Kategori ve Birim Tipi zorunludur.";
        public const string MasterMaterialRequiredForMergeMessage = "Birleştirme işlemi için Master Malzeme seçilmelidir.";
        public const string MergedAsAliasMessage = "Alias olarak birleştirildi.";
        public const string RequestRejectedMessage = "Alias olarak birleştirildi.";
        public const string RequestResolvedAndInventoryUpdatedMessage = "Talep başarıyla çözümlendi ve kullanıcının envanteri güncellendi.";
        public const string InventoryItemNotFoundMessage = "Dolabınızda böyle bir ürün bulunamadı.";
        public const string IngredientDeletedMessage = "Ürün dolabınızdan başarıyla silindi.";
        public const string NoExpiredProductsToCleanMessage = "Temizlenecek tarihi geçmiş ürün bulunamadı.";
        public const string IngredientRequestNotFoundMessage = "Talep bulunamadı veya bu talebi iptal etme yetkiniz yok.";
        public const string RequestAlreadyProcessedCannotBeCancelledMessage = "Bu talep zaten yönetici tarafından incelenmiş veya karara bağlanmış. İptal edilemez.";
        public const string IngredientRequestCancelledMessage = "Talebiniz başarıyla iptal edildi ve dolabınızdan kaldırıldı.";
        public const string RequestCancelledByUserMessage = "Kullanıcı talebini kendi isteğiyle iptal etti/geri çekti.";
    }
}
