namespace OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types
{
    public class InternalException : Exception
    {
        public InternalException(string message) : base(message) { }
    }
}
