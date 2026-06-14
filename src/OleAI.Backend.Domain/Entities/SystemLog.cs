using OleAI.Backend.Core.Repositories;

namespace OleAI.Backend.Domain.Entities
{
    public class SystemLog : Entity<int>
    {
        public string? Message { get; private set; }
        public string? MessageTemplate { get; private set; }
        public string? Level { get; private set; }
        public DateTime? TimeStamp { get; private set; }
        public string? Exception { get; private set; }
        public string? Properties { get; private set; }
        public string? CorrelationId { get; private set; }

        protected SystemLog()
        {
        }
    }
}
