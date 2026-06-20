using OleAI.Backend.Core.Repositories;

namespace OleAI.Backend.Domain.Entities
{
    /// <summary>
    /// Sistemde tanımlı temel diyet kısıtlarını temsil eder (Örn: Vegan, Glütensiz).
    /// </summary>
    public class DietaryPreference : Entity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string IconName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public virtual ICollection<UserDietaryPreference> UserDietaryPreferences { get; set; }

        // dışlanacak malzeme ya da kategoriler NOT: NULLABLE OLABİLİR
        public virtual ICollection<DietaryPreferenceExcludedIngredient> ExcludedIngredients { get; set; }
        public virtual ICollection<DietaryPreferenceExcludedCategory> ExcludedCategories { get; set; }
        public DietaryPreference()
        {
            UserDietaryPreferences = new HashSet<UserDietaryPreference>();
            ExcludedIngredients = new HashSet<DietaryPreferenceExcludedIngredient>();
            ExcludedCategories = new HashSet<DietaryPreferenceExcludedCategory>();
        }
    }
}
