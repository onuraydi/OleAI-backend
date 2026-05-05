using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Persistence.Contexts;

namespace OleAI.Backend.Persistence.Repositories
{
    public class DietaryPreferenceRepository : EfRepositoryBase<DietaryPreference, Guid>, IDietaryPreferenceRepository
    {
        public DietaryPreferenceRepository(AppDbContext context) : base(context) { }
    }
}
