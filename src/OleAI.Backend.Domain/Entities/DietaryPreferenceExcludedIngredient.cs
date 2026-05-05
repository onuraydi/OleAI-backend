namespace OleAI.Backend.Domain.Entities
{
    public class DietaryPreferenceExcludedIngredient
    {
        public Guid DietaryPreferenceId { get; set; }
        public virtual DietaryPreference DietaryPreference { get; set; } = null!;

        public Guid IngredientId { get; set; }
        public virtual Ingredient Ingredient { get; set; }
    }
}
