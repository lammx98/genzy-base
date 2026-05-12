namespace Genzy.Base.Exceptions

{

    public class ValidationException : AppException

    {

        public ValidationException() : base(AppErrorCodes.ValidationFailed, "Validation failed", null) { }



        public ValidationException(string message) : base(AppErrorCodes.ValidationFailed, message, null) { }



        public ValidationException(string? errorCode, string? message, object? detail = null)

            : base(errorCode ?? AppErrorCodes.ValidationFailed, message ?? "Validation failed", detail) { }

    }

}

