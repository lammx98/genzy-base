namespace Genzy.Base.Wrappers
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Result { get; set; }
        public object? Error { get; set; }

        public ApiResponse(T? data = default, string? message = null)
        {
            Success = true;
            Message = message;
            Result = data;
        }

        public ApiResponse(string? message, object? error = null)
        {
            Success = false;
            Message = message;
            Error = error;
        }

        public static ApiResponse<T> New(T? data, string? message = null) => new(data, message);
        public static ApiResponse<T> Fail(string? message, object? error = null) => new(message, error);
    }

    public class ApiResponse : ApiResponse<object>
    {
        public ApiResponse(object? data = null, string? message = null) : base(data, message) { }
        public ApiResponse(string? message, object? error = null) : base(message, error) { }
    }
}
