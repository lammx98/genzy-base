namespace Genzy.Base.Exceptions
{
    public class BadException : AppException
    {
        public BadException() { }
        public BadException(string message = "Bad Request", object? detail = null)
        : base(400, message, detail) { }
    }
}