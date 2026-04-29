namespace OleAI.Backend.Domain
{
    public static class DomainConstants
    {
        public const string InvalidConsumptionAmountMessage = "Tüketilecek miktar 0'dan büyük olmalıdır.";
        public const string InsufficientIngredientMessage = "Dolapta yeterli miktar yok.";
        public const string InvalidAdditionAmountMessage = "Eklenecek miktar 0'dan büyük olmalıdır.";
        public const string ProductAlreadyApprovedOrRequestIdMissingMessage = "Bu ürün zaten onaylı veya talep ID'si eksik.";
        public const string AmountCannotBeNegativeMessage = "Miktar 0'dan küçük olamaz.";
        public const string ProductAlreadyPurchasedMessage = "Bu ürün zaten satın alındı olarak işaretlenmiş.";
        public const string CannotChangePurchasedProductQuantity = "Satın alınmış bir ürünün miktarı değiştirilemez.";
        public const string InvalidProductQuantityInList = "Alışveriş listesindeki ürün miktarı sıfırdan büyük olmalıdır.";
        public const string OnlyLlmApprovedRecipesCanBeApprovedByAdmin = "Sadece LLM onayından geçmiş tarifler admin tarafından onaylanabilir.";
    }
}
