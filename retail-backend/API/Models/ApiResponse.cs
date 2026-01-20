namespace API.Models;

/// <summary>
/// Standard API response wrapper for successful responses
/// </summary>
/// <typeparam name="T">Type of data being returned</typeparam>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public DateTime Timestamp { get; set; }

    public ApiResponse()
    {
        Success = true;
        Timestamp = DateTime.UtcNow;
    }

    public static ApiResponse<T> SuccessResponse(T data, string message = "تمت العملية بنجاح")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ApiResponse<T> FailureResponse(string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default,
            Timestamp = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Standard API response wrapper for error responses
/// </summary>
public class ApiErrorResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public List<string>? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiErrorResponse Create(string message, List<string>? errors = null)
    {
        return new ApiErrorResponse
        {
            Success = false,
            Message = message,
            Errors = errors,
            Timestamp = DateTime.UtcNow
        };
    }
}
