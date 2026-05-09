namespace OleAI.Backend.Application.Services.UserProfiles
{
    public interface IUserProfileVectorService
    {
        /// <summary>
        /// Kullanıcının tarifle olan etkileşimine göre (Like, Save, Comment) 
        /// AI profil vektörünü asenkron olarak günceller.
        /// </summary>
        Task UpdateProfileVectorAsync(Guid userId, Guid recipeId, string interactionType, CancellationToken cancellationToken);

        /// <summary>
        /// Kullanıcının diyet tercihleri veya alerji bilgileri değiştiğinde
        /// AI profil vektörünü baştan günceller.
        /// </summary>
        Task UpdateDietaryProfileAsync(Guid userId, CancellationToken cancellationToken);
    }
}
