using OleAI.Backend.Core.Repositories;

namespace OleAI.Backend.Domain.Entities
{
    public class UserOperationClaim : Entity<Guid>
    {
        public Guid UserId { get; set; }
        public Guid OperationClaimId { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual OperationClaim operationClaim { get; set; } = null!;
    }
}
