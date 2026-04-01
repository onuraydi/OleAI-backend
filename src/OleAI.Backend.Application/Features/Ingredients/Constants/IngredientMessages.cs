namespace OleAI.Backend.Application.Features.Ingredients.Constants
{
    public static class IngredientMessages
    {
        public const string IngredientNotFoundMessage = "İstenilen malzeme bulunamadı.";
        public const string ActiveIngredientAlreadyExistsMessage = "Bu isimde aktif bir malzeme zaten mevcut.";
        public const string IngredientUpdatedMessage = "Malzeme güncellendi.";
        public const string IngredientSoftDeletedDueToInventoryUsageMessage = "Bu malzeme kullanıcıların dolabında bulunduğu için kalıcı olarak silinemedi, ancak sistemde 'Pasif' (Soft Delete) duruma getirildi.";
        public const string IngredientDeletedMessage = "Malzeme başarıyla silindi.";
    }
}
