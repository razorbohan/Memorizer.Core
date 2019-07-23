namespace Memorizer.Web.Models
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; }

        public ApiResponse(bool success,  string error = null)
        {
            Success = success;
            Error = error;
        }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T Body { get; set; }

        public ApiResponse(bool success, T body = default(T), string error = null) : base(success, error)
        {
            Body = body;
        }
    }

    public class AuthBody
    {
        public string Username;
        public string Token;
    }
}
