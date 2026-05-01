
namespace OleAI.Backend.Application.Services.UserProfiles
{
    public class DummyUserProfileVectorService : IUserProfileVectorService
    {
        public Task UpdateProfileVectorAsync(Guid userId, Guid recipeId, string interactionType, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
