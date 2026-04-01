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
    }
}
