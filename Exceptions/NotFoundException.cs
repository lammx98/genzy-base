namespace Genzy.Base.Exceptions

{

    public class NotFoundException : AppException

    {

        public NotFoundException() : base(AppErrorCodes.NotFound, "Resource not found", null) { }



        public NotFoundException(string message) : base(AppErrorCodes.NotFound, message, null) { }



        public NotFoundException(string? errorCode, string? message, object? detail = null)

            : base(errorCode ?? AppErrorCodes.NotFound, message ?? "Resource not found", detail) { }

    }

}

