namespace OleAI.Backend.Domain.Entities
{
    public class UserDietaryPreference
    {
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public Guid DietaryPreferenceId { get; set; }
        public virtual DietaryPreference DietaryPreference { get; set; } = null!;
    }
}
