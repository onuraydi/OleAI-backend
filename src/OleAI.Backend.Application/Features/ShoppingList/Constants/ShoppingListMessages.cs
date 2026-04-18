namespace OleAI.Backend.Application.Features.ShoppingList.Constants;

public static class ShoppingListMessages
{
    public const string ProductAlreadyInList = "Bu ürün zaten alışveriş listenizde bulunuyor.";
    public const string ItemNotFoundOrUnauthorized = "Alışveriş listesinde böyle bir ürün bulunamadı veya bu işlem için yetkiniz yok.";
    public const string ItemMarkedAsPurchased = "Ürün satın alındı olarak işaretlendi ve dolabınıza eklendi.";
    public const string BulkMarkedAsPurchased = "{0} ürün satın alındı olarak işaretlendi ve dolabınıza eklendi.";
    public const string ItemRemovedFromList = "Ürün alışveriş listenizden kaldırıldı.";
    public const string ItemAddedToList = "Ürün alışveriş listenize eklendi.";
    public const string IngredientNotFound = "Eklemek istediğiniz malzeme sistemde bulunamadı.";
    public const string ItemUpdated = "Alışveriş listesi ürünü güncellendi.";
    public const string AlreadyPurchased = "Bu ürün zaten satın alındı olarak işaretlenmiş.";
}
