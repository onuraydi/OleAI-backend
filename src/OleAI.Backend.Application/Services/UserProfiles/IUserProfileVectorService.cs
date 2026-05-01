namespace OleAI.Backend.Application.Services.UserProfiles
{
    public interface IUserProfileVectorService
    {
        /// <summary>
        /// Kullanıcının tarifle olan etkileşimine göre (Like, Save, Comment) 
        /// AI profil vektörünü asenkron olarak günceller.
        /// </summary>
        Task UpdateProfileVectorAsync(Guid userId, Guid recipeId, string interactionType, CancellationToken cancellationToken);
    }
}
