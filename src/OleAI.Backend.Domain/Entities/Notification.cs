using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Domain.Entities
{
    public class Notification : Entity<Guid>
    {
        public Guid UserId { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public NotificationType Type { get; private set; }
        public Guid? RelatedEntityId { get; private set; }
        public bool IsRead { get; private set; }

        public virtual User User { get; private set; } = null!;

        protected Notification() { }

        public Notification(Guid id, Guid userId, string title, string message, NotificationType type, Guid? relatedEntityId = null)
        {
            Id = id;
            UserId = userId;
            Title = title;
            Message = message;
            Type = type;
            RelatedEntityId = relatedEntityId;
            IsRead = false;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}
