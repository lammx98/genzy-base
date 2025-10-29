namespace Genzy.Base.Exceptions
{
    public class ForbiddenException : AppException
    {
        public ForbiddenException() { }
        public ForbiddenException(string message = "Forbidden", object? detail = null)
            : base(403, message, detail) { }
    }
}