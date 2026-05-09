namespace OleAI.Backend.Application.Features.Users.Dtos.Responses
{
    public class UserProfileResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsEmailVerified { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<DietaryPreferenceItemResponse> DietaryPreferences { get; set; } = new();
        public List<AllergyRestrictionItemResponse> Allergies { get; set; } = new();
        public List<DislikedIngredientItemResponse> DislikedIngredients { get; set; } = new();
    }
}
