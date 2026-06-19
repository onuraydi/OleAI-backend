using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Domain.Enums;
using OleAI.Backend.Domain.Services;

namespace OleAI.Backend.Domain.Entities
{
    public class Ingredient : Entity<Guid>
    {
        public string Name { get; private set; }
        public Guid CategoryId { get; private set; }
        public bool IsPolarizing { get; private set; }

        /// <summary>
        /// Temel Gıda mı? (Tuz, Yağ, Su vb. her evde bulunduğu varsayılan malzemeler)
        /// Eğer true ise yapay zeka öneri motoru bunu hesaba katmaz ve tüketim işleminde dolaptan düşülmez.
        /// </summary>
        public bool IsStaple { get; private set; }

        /// <summary>
        /// Malzemenin ait olduğu ölçü birimi ailesi.
        /// Örn: Weight → (Gram, Kilogram), Volume → (Milliliter, Liter), Count → (Piece, Bunch, ...)
        /// Kullanıcıların bu aile içindeki herhangi bir birimde ölçüm yapmasına izin verilir.
        /// </summary>
        public MeasurementCategory UnitFamily { get; private set; }

        /// <summary>
        /// Malzemenin açılmış / normal şartlardaki ortalama raf ömrü (gün cinsinden).
        /// AI tarafından tahmin edilebilir veya manuel girilebilir.
        /// </summary>
        public int? EstimatedShelfLifeInDays { get; private set; }

        /// <summary>
        /// Admin panelinden belirlenen varsayılan son kullanma tarihi takip türü (Örn: Auto, ShelfLife, ExactDate)
        /// </summary>
        public ExpirationMode DefaultExpirationMode { get; private set; }

        public virtual Category Category { get; private set; } = null!;
        public virtual ICollection<IngredientAlias> Aliases { get; private set; }
        public virtual ICollection<UserIngredient> UserIngredients { get; private set; }
        public virtual ICollection<ShoppingListItem> ShoppingListItems { get; private set; }
        public virtual ICollection<UserDislikedIngredient> UserDislikedIngredients { get; private set; }

        protected Ingredient()
        {
            Name = string.Empty;
            UnitFamily = MeasurementCategory.Other;
            DefaultExpirationMode = ExpirationMode.Auto;
            Aliases = new HashSet<IngredientAlias>();
            UserIngredients = new HashSet<UserIngredient>();
            ShoppingListItems = new HashSet<ShoppingListItem>();
            UserDislikedIngredients = new HashSet<UserDislikedIngredient>();
        }

        public Ingredient(Guid id, string name, Guid categoryId, MeasurementCategory unitFamily = MeasurementCategory.Other, int? estimatedShelfLifeInDays = null, ExpirationMode defaultExpirationMode = ExpirationMode.Auto, bool isStaple = false)
            : this()
        {
            Id = id;
            Name = name;
            CategoryId = categoryId;
            UnitFamily = unitFamily;
            EstimatedShelfLifeInDays = estimatedShelfLifeInDays;
            DefaultExpirationMode = defaultExpirationMode;
            IsStaple = isStaple;
        }

        public void SetStaple(bool isStaple)
        {
            IsStaple = isStaple;
        }

        /// <summary>
        /// Malzemenin ölçü birimi ailesini günceller.
        /// </summary>
        public void SetUnitFamily(MeasurementCategory unitFamily)
        {
            UnitFamily = unitFamily;
        }

        /// <summary>
        /// Verilen birimin bu malzeme için geçerli olup olmadığını kontrol eder.
        /// </summary>
        public bool IsUnitCompatible(MeasurementUnit unit)
            => MeasurementUnitHelper.IsCompatible(UnitFamily, unit);

        /// <summary>
        /// Kutuplaştırıcı (çok sevilmeyen) malzeme durumunu günceller.
        /// </summary>
        public void SetPolarizing(bool isPolarizing)
        {
            IsPolarizing = isPolarizing;
        }

        /// <summary>
        /// Ortalama raf ömrünü günceller.
        /// </summary>
        public void SetEstimatedShelfLifeInDays(int? days)
        {
            EstimatedShelfLifeInDays = days;
        }

        public void SetDefaultExpirationMode(ExpirationMode mode)
        {
            DefaultExpirationMode = mode;
        }
    }
}
