using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Domain.Services
{
    /// <summary>
    /// MeasurementCategory ile MeasurementUnit arasındaki aile/uyumluluk ilişkisini yönetir.
    /// Domain katmanında unit validasyonu için tek yetkili kaynaktır.
    /// </summary>
    public static class MeasurementUnitHelper
    {
        private static readonly IReadOnlyDictionary<MeasurementCategory, IReadOnlySet<MeasurementUnit>> _families =
            new Dictionary<MeasurementCategory, IReadOnlySet<MeasurementUnit>>
            {
                [MeasurementCategory.Weight] = new HashSet<MeasurementUnit>
                {
                    MeasurementUnit.Gram,
                    MeasurementUnit.Kilogram
                },
                [MeasurementCategory.Volume] = new HashSet<MeasurementUnit>
                {
                    MeasurementUnit.Milliliter,
                    MeasurementUnit.Liter
                },
                [MeasurementCategory.Count] = new HashSet<MeasurementUnit>
                {
                    MeasurementUnit.Piece,
                    MeasurementUnit.Bunch,
                    MeasurementUnit.Slice,
                    MeasurementUnit.Packet,
                    MeasurementUnit.Pinch
                },
                [MeasurementCategory.Other] = new HashSet<MeasurementUnit>(
                    Enum.GetValues<MeasurementUnit>()
                )
            };

        /// <summary>
        /// Verilen <paramref name="unit"/> değerinin belirtilen <paramref name="category"/> ailesine
        /// uygun olup olmadığını kontrol eder.
        /// </summary>
        public static bool IsCompatible(MeasurementCategory category, MeasurementUnit unit)
        {
            if (!_families.TryGetValue(category, out var units))
            {
                // Enum was mapped to an undefined integer (e.g. legacy cache or DB value)
                return _families[MeasurementCategory.Other].Contains(unit);
            }
            return units.Contains(unit);
        }

        /// <summary>
        /// Verilen <paramref name="category"/> için geçerli tüm birimleri döner.
        /// </summary>
        public static IReadOnlySet<MeasurementUnit> GetUnitsForCategory(MeasurementCategory category)
        {
            if (_families.TryGetValue(category, out var units))
                return units;
            return _families[MeasurementCategory.Other];
        }

        /// <summary>
        /// Bir kategori için semantik olarak en anlamlı varsayılan birimi döner.
        /// Otomatik oluşturulan kayıtlarda (örn. ShoppingList auto-add) kullanılır.
        /// </summary>
        public static MeasurementUnit GetDefaultUnitForCategory(MeasurementCategory category)
            => category switch
            {
                MeasurementCategory.Weight => MeasurementUnit.Gram,
                MeasurementCategory.Volume => MeasurementUnit.Liter,
                MeasurementCategory.Count => MeasurementUnit.Piece,
                _ => MeasurementUnit.Piece
            };

        /// <summary>
        /// Miktarı bir birimden diğerine çevirir (sadece ağırlık ve hacim için uyumludur).
        /// </summary>
        public static double ConvertAmount(double amount, MeasurementUnit fromUnit, MeasurementUnit toUnit)
        {
            if (fromUnit == toUnit) return amount;

            if (fromUnit == MeasurementUnit.Kilogram && toUnit == MeasurementUnit.Gram) return amount * 1000;
            if (fromUnit == MeasurementUnit.Gram && toUnit == MeasurementUnit.Kilogram) return amount / 1000;

            if (fromUnit == MeasurementUnit.Liter && toUnit == MeasurementUnit.Milliliter) return amount * 1000;
            if (fromUnit == MeasurementUnit.Milliliter && toUnit == MeasurementUnit.Liter) return amount / 1000;

            return amount;
        }
    }
}
