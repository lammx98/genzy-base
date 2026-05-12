namespace Genzy.Base.Exceptions
{
    public class ForbiddenException : AppException
    {
        public ForbiddenException() : base(AppErrorCodes.Forbidden, "Forbidden", null) { }

        public ForbiddenException(string message) : base(AppErrorCodes.Forbidden, message, null) { }

        public ForbiddenException(string? errorCode, string? message, object? detail = null)
            : base(errorCode ?? AppErrorCodes.Forbidden, message ?? "Forbidden", detail) { }
    }
}
