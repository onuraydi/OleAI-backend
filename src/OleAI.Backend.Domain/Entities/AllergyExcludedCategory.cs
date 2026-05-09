namespace OleAI.Backend.Domain.Entities
{
    public class AllergyExcludedCategory
    {
        public Guid AllergyRestrictionId { get; set; }
        public virtual AllergyRestriction AllergyRestriction { get; set; } = null!;

        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
