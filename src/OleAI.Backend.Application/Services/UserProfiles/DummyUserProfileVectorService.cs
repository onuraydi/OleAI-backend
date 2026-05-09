
namespace OleAI.Backend.Application.Services.UserProfiles
{
    public class DummyUserProfileVectorService : IUserProfileVectorService
    {
        public Task UpdateProfileVectorAsync(Guid userId, Guid recipeId, string interactionType, CancellationToken cancellationToken)
        {
            // Gerçek implementasyon Python AI servisi hazır olduğunda eklenecektir.
            return Task.CompletedTask;
        }

        public Task UpdateDietaryProfileAsync(Guid userId, CancellationToken cancellationToken)
        {
            // Gerçek implementasyon Python AI servisi hazır olduğunda eklenecektir.
            return Task.CompletedTask;
        }
    }
}
