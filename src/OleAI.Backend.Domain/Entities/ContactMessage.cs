using OleAI.Backend.Core.Repositories;

namespace OleAI.Backend.Domain.Entities
{
    public class ContactMessage : Entity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;

        public ContactMessage() { }

        public ContactMessage(Guid id, string name, string email, string department, string message, bool isRead)
        {
            Id = id;
            Name = name;
            Email = email;
            Department = department;
            Message = message;
            IsRead = isRead;
        }
    }
}
