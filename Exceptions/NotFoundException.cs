namespace Genzy.Base.Exceptions
{
    public class NotFoundException : AppException
    {
        public NotFoundException() { }
        public NotFoundException(string message = "Resource not found", object? detail = null)
        : base(404, message, detail) { }
    }
}