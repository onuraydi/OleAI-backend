using OleAI.Backend.Core.Repositories;

namespace OleAI.Backend.Domain.Entities
{
    public class OperationClaim : Entity<Guid>
    {
        public string Name { get; set; } = string.Empty;
    }
}
