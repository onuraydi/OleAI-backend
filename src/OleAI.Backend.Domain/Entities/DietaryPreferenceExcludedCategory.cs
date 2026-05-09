namespace OleAI.Backend.Domain.Entities
{
    public class DietaryPreferenceExcludedCategory
    {
        public Guid DietaryPreferenceId { get; set; }
        public virtual DietaryPreference DietaryPreference { get; set; } = null!;

        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
