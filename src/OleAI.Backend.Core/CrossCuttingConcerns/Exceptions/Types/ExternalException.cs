namespace OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types
{
    public class ExternalException : Exception
    {
        public ExternalException(string message) : base(message) { }
    }
}
