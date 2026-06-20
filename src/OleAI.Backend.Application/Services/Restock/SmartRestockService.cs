using System;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Services.Restock
{
    public class SmartRestockService : ISmartRestockService
    {
        public double? CalculateRestockAmount(double requiredAmount, double pantryAmount, MeasurementUnit unit)
        {
            double missingAmount = requiredAmount - pantryAmount;

            // Eğer dolapta fazlasıyla varsa, alınacak bir şey yoktur.
            if (missingAmount <= 0)
                return null;

            // --- 1. GÖRMEZDEN GELME (TÖLERANS) KONTROLÜ ---
            // Yüzdesel tolerans: Eksik miktar, tarifin istediğinin %5'inden küçük mü?
            bool isUnderPercentageTolerance = (missingAmount / requiredAmount) <= 0.05;

            // Mutlak tolerans: Gram ve Mililitre için 10 birimin altındaki eksiklikler önemsizdir.
            bool isUnderAbsoluteTolerance = false;
            if (unit == MeasurementUnit.Gram || unit == MeasurementUnit.Milliliter)
            {
                if (missingAmount <= 10)
                    isUnderAbsoluteTolerance = true;
            }

            // Eğer her iki toleranstan birine uyuyorsa alışverişe gerek yok
            if (isUnderPercentageTolerance || isUnderAbsoluteTolerance)
                return null;

            // --- 2. AKILLI AMBALAJ YUVARLAMASI ---

            switch (unit)
            {
                case MeasurementUnit.Gram:
                    // En küçük paket 250g. Daha fazlaysa 250g'ın katlarına (250, 500, 750, 1000...) yukarı yuvarla.
                    return RoundUpToMultiplier(missingAmount, 250.0);

                case MeasurementUnit.Milliliter:
                    // En küçük sıvı paket 500ml. Daha fazlaysa 500ml'nin katlarına yuvarla.
                    return RoundUpToMultiplier(missingAmount, 500.0);

                case MeasurementUnit.Kilogram:
                case MeasurementUnit.Liter:
                    // Kg ve Litre için genelde 1 birim veya 0.5 (yarım kilo) şeklinde satılır.
                    // Biz 0.5 ve katlarına yukarı yuvarlayalım (0.5 kg, 1 kg, 1.5 kg).
                    return RoundUpToMultiplier(missingAmount, 0.5);

                case MeasurementUnit.Piece:
                default:
                    // Adet veya kaşık gibi ölçüler sadece "tam sayı" olmalıdır.
                    // Eksik miktar 0.3 dahi olsa (örneğin 0.3 yumurta) 1 adete yukarı yuvarla.
                    return Math.Ceiling(missingAmount);
            }
        }

        private double RoundUpToMultiplier(double value, double multiplier)
        {
            if (value <= 0) return 0;
            return Math.Ceiling(value / multiplier) * multiplier;
        }
    }
}
