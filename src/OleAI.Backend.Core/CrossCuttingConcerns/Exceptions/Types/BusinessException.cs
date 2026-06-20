namespace OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types
{
    public class BusinessException : Exception
    {
        public string? ErrorCode { get; set; }

        public BusinessException(string message, string? errorCode = null) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
