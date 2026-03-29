namespace OleAI.Backend.Domain
{
    public static class DomainConstants
    {
        public const string InvalidConsumptionAmountMessage = "Tüketilecek miktar 0'dan büyük olmalıdır.";
        public const string InsufficientIngredientMessage = "Dolapta yeterli miktar yok.";
        public const string InvalidAdditionAmountMessage = "Eklenecek miktar 0'dan büyük olmalıdır.";
        public const string ProductAlreadyApprovedOrRequestIdMissingMessage = "Bu ürün zaten onaylı veya talep ID'si eksik.";
    }
}
