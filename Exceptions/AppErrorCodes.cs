namespace Genzy.Base.Exceptions
{
    /// <summary>
    /// Stable string codes for API errors (distinct from HTTP status).
    /// </summary>
    public static class AppErrorCodes
    {
        public const string InternalError = "INTERNAL_ERROR";
        public const string BadRequest = "BAD_REQUEST";
        public const string ValidationFailed = "VALIDATION_FAILED";
        public const string Unauthorized = "UNAUTHORIZED";
        public const string Forbidden = "FORBIDDEN";
        public const string NotFound = "NOT_FOUND";

        public const string ArgumentNull = "ARGUMENT_NULL";
        public const string ArgumentInvalid = "ARGUMENT_INVALID";
        public const string InvalidOperation = "INVALID_OPERATION";
        public const string NotImplemented = "NOT_IMPLEMENTED";
        public const string Timeout = "TIMEOUT";
        public const string UnauthorizedAccess = "UNAUTHORIZED_ACCESS";
    }
}
