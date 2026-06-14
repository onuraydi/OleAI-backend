using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Persistence.Contexts;

namespace OleAI.Backend.Persistence.Repositories
{
    public class SystemLogRepository : EfRepositoryBase<SystemLog, int>, ISystemLogRepository
    {
        public SystemLogRepository(AppDbContext context) : base(context)
        {
        }
    }
}
