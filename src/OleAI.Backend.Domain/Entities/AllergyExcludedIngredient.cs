namespace OleAI.Backend.Domain.Entities
{
    public class AllergyExcludedIngredient
    {
        public Guid AllergyRestrictionId { get; set; }
        public virtual AllergyRestriction AllergyRestriction { get; set; } = null!;

        public Guid IngredientId { get; set; }
        public virtual Ingredient Ingredient { get; set; }
    }
}
