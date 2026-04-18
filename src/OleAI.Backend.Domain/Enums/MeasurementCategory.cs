namespace OleAI.Backend.Domain.Enums
{
    /// <summary>
    /// Bir malzemenin hangi ölçü birimleri ailesine ait olduğunu tanımlar.
    /// Bu sayede bir malzeme için aynı aile içindeki tüm birimler geçerli kabul edilir.
    /// Örn: Weight → Gram, Kilogram; Volume → Milliliter, Liter
    /// </summary>
    public enum MeasurementCategory
    {
        Weight = 1,
        Volume = 2,
        Count = 3,
        Other = 99
    }
}
