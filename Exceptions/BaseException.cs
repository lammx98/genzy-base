namespace Genzy.Base.Exceptions
{
    public class AppException : Exception
    {
        public int ErrorCode { get; }
        public object? Detail { get; set; }

        public AppException() { }

        public AppException(int? errorCode, string? message, object? detail = null) : base(message)
        {
            ErrorCode = errorCode ?? 500;
            Detail = detail;
        }
    }
}