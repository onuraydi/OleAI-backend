using OleAI.Backend.Core.Repositories;

namespace OleAI.Backend.Domain.Entities
{
    public class User : Entity<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public string? ProfileImageUrl { get; set; }
        public bool IsOnboarded { get; set; } = false;

        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

        public bool IsEmailVerified { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public bool TermsAccepted { get; set; } = false;

        public int FailedLoginAttempts { get; set; } = 0;
        public DateTime? LockoutEnd { get; set; }

        public virtual ICollection<UserOperationClaim> UserOperationClaims { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        public virtual ICollection<UserDietaryPreference> UserDietaryPreferences { get; set; } = new HashSet<UserDietaryPreference>();
        public virtual ICollection<UserAllergy> UserAllergies { get; set; } = new HashSet<UserAllergy>();
        public virtual ICollection<UserDislikedIngredient> UserDislikedIngredients { get; set; } = new HashSet<UserDislikedIngredient>();
        public virtual ICollection<UserIngredient> UserIngredients { get; set; } = new HashSet<UserIngredient>();
        public virtual ICollection<RecipeInteraction> Interactions { get; set; } = new HashSet<RecipeInteraction>();
        public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
        public virtual ICollection<UserDevice> UserDevices { get; set; } = new HashSet<UserDevice>();

        public User()
        {
            UserOperationClaims = new HashSet<UserOperationClaim>();
            RefreshTokens = new HashSet<RefreshToken>();
        }
    }
}
