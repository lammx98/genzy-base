namespace Genzy.Base.Exceptions
{
    public class ValidationException : AppException
    {
        public ValidationException() { }
        public ValidationException(string message = "Validation failed", object? detail = null)
            : base(400, message, detail) { }
    }
}