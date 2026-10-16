namespace NMC.Client.Services
{
    public class ApiResult
    {
        public bool IsSuccess { get; private init; }
        public string ErrorMessage { get; private init; } = string.Empty;

        public static ApiResult Success()
        {
            return new ApiResult { IsSuccess = true };
        }

        public static ApiResult Failure(string errorMessage)
        {
            return new ApiResult { IsSuccess = false, ErrorMessage = errorMessage };
        }
    }

    public class ApiResult<T>
    {
        public bool IsSuccess { get; private init; }
        public T? Data { get; private init; }
        public string ErrorMessage { get; private init; } = string.Empty;

        public static ApiResult<T> Success(T data)
        {
            return new ApiResult<T> { IsSuccess = true, Data = data };
        }

        public static ApiResult<T> Failure(string errorMessage)
        {
            return new ApiResult<T> { IsSuccess = false, ErrorMessage = errorMessage };
        }
    }
}
