namespace Genzy.Base.Exceptions

{

    public class AppException : Exception

    {

        public string? ErrorCode { get; }

        public object? Detail { get; set; }



        public AppException() { }



        /// <summary>Chỉ message (không mã lỗi nghiệp vụ).</summary>

        public AppException(string message) : this(null, message, null) { }



        public AppException(string? errorCode, string? message, object? detail = null)

            : base(message ?? string.Empty)

        {

            ErrorCode = errorCode;

            Detail = detail;

        }

    }

}

