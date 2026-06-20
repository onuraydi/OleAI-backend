using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Domain.Entities
{
    public class UserDevice : Entity<Guid>
    {
        public Guid UserId { get; private set; }
        public string DeviceToken { get; private set; }
        public DevicePlatform Platform { get; private set; }
        public DateTime LastActiveAt { get; private set; }

        public virtual User User { get; private set; } = null!;

        protected UserDevice() { }

        public UserDevice(Guid id, Guid userId, string deviceToken, DevicePlatform platform)
        {
            Id = id;
            UserId = userId;
            DeviceToken = deviceToken;
            Platform = platform;
            LastActiveAt = DateTime.UtcNow;
        }

        public void UpdateLastActiveAt()
        {
            LastActiveAt = DateTime.UtcNow;
        }
    }
}
