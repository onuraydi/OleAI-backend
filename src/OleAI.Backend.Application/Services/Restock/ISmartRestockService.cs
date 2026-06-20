using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Services.Restock
{
    public interface ISmartRestockService
    {
        /// <summary>
        /// Eksik malzeme miktarını (Missing Amount) tolerans ve paketleme mantığına göre analiz eder.
        /// Çok küçük eksiklikler (tolerans altı) için null döner (alışverişe eklenmesine gerek yok).
        /// Eklenmesi gereken durumlarda mantıklı ambalaj miktarına yuvarlayıp döner.
        /// </summary>
        /// <param name="requiredAmount">Tarifin istediği orijinal miktar</param>
        /// <param name="pantryAmount">Kullanıcının dolabında var olan miktar</param>
        /// <param name="unit">Ölçü Birimi</param>
        /// <returns>Alışveriş listesine eklenecek mantıklı miktar, veya eklenmemesi gerekiyorsa null</returns>
        double? CalculateRestockAmount(double requiredAmount, double pantryAmount, MeasurementUnit unit);
    }
}
