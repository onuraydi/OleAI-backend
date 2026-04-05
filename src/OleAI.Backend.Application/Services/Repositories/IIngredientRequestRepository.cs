using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Services.Repositories
{
    public interface IIngredientRequestRepository : IAsyncRepository<IngredientRequest, Guid>
    {
    }
}
