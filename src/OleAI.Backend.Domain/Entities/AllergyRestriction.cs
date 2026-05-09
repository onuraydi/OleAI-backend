using OleAI.Backend.Core.Repositories;

namespace OleAI.Backend.Domain.Entities
{
    public class AllergyRestriction : Entity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual ICollection<UserAllergy> UserAllergies { get; set; }

        // Dışlama
        public virtual ICollection<AllergyExcludedIngredient> ExcludedIngredients { get; set; }
        public virtual ICollection<AllergyExcludedCategory> ExcludedCategories { get; set; }
        public AllergyRestriction()
        {
            UserAllergies = new HashSet<UserAllergy>();
            ExcludedIngredients = new HashSet<AllergyExcludedIngredient>();
            ExcludedCategories = new HashSet<AllergyExcludedCategory>();
        }
    }
}
