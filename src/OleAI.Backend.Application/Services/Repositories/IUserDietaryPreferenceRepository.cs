using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Services.Repositories
{
    /// <summary>
    /// UserDietaryPreference join tablosu için repository arayüzü.
    /// Entity&lt;TId&gt; base sınıfından türemediği için IAsyncRepository kullanılamaz;
    /// bu nedenle yalnızca ihtiyaç duyulan operasyonlar açılmıştır.
    /// </summary>
    public interface IUserDietaryPreferenceRepository
    {
        Task<int> ExecuteDeleteByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<UserDietaryPreference> entities, CancellationToken cancellationToken = default);
    }
}
