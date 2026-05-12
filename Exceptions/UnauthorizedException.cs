namespace Genzy.Base.Exceptions

{

    public class UnauthorizedException : AppException

    {

        public UnauthorizedException() : base(AppErrorCodes.Unauthorized, "Unauthorized access", null) { }



        public UnauthorizedException(string message) : base(AppErrorCodes.Unauthorized, message, null) { }



        public UnauthorizedException(string? errorCode, string? message, object? detail = null)

            : base(errorCode ?? AppErrorCodes.Unauthorized, message ?? "Unauthorized access", detail) { }

    }

}

