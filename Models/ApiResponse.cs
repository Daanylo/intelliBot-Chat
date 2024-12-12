namespace intelliBot.Models;

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public bool Success { get; set; }

    public ApiResponse(T? data, string? errorMessage = null)
    {
        Data = data;
        ErrorMessage = errorMessage;
        Success = errorMessage == null;
    }
}