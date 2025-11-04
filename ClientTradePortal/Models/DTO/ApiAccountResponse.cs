namespace ClientTradePortal.Models.DTO;

public class ApiAccountResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();
}