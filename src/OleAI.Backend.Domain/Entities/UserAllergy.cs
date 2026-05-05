namespace OleAI.Backend.Domain.Entities
{
    public class UserAllergy
    {
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public Guid AllergyRestrictionId { get; set; }
        public virtual AllergyRestriction AllergyRestriction { get; set; } = null!;
    }
}
