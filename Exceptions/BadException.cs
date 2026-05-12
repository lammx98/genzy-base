namespace Genzy.Base.Exceptions

{

    public class BadException : AppException

    {

        public BadException() : base(AppErrorCodes.BadRequest, "Bad Request", null) { }



        public BadException(string message) : base(AppErrorCodes.BadRequest, message, null) { }



        public BadException(string? errorCode, string? message, object? detail = null)

            : base(errorCode ?? AppErrorCodes.BadRequest, message ?? "Bad Request", detail) { }

    }

}

