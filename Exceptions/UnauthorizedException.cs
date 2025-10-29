namespace Genzy.Base.Exceptions
{
    public class UnauthorizedException : AppException
    {
        public UnauthorizedException() { }
        public UnauthorizedException(string message = "Unauthorized access", object? detail = null)
            : base(401, message, detail) { }
    }
}