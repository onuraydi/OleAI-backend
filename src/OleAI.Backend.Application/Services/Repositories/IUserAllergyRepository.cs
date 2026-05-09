using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Services.Repositories
{
    /// <summary>
    /// UserAllergy join tablosu için repository arayüzü.
    /// Entity&lt;TId&gt; base sınıfından türemediği için IAsyncRepository kullanılamaz;
    /// bu nedenle yalnızca ihtiyaç duyulan operasyonlar açılmıştır.
    /// </summary>
    public interface IUserAllergyRepository
    {
        Task<int> ExecuteDeleteByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<UserAllergy> entities, CancellationToken cancellationToken = default);
    }
}
