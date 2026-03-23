using OleAI.Backend.Core.Repositories;

namespace OleAI.Backend.Domain.Entities
{
    public class RefreshToken : Entity<Guid>
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public string CreatedByIp { get; set; } = string.Empty;
        public DateTime? Revoked { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReplacedByToken { get; set; }
        public string? ReasonRevoked { get; set; }

        public bool IsExpired => DateTime.UtcNow >= Expires;
        public bool IsRevoked => Revoked != null;
        public bool IsActive => !IsRevoked && !IsExpired;

        public virtual User User { get; set; } = null!;
    }
}
