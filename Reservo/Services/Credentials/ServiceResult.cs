namespace Reservo.Services.Credentials
{
    public class ServiceResult
    {
        public bool Success { get; init; }
        public string Message { get; init; } = string.Empty;

        public static ServiceResult Ok(string message = "") => new() { Success = true, Message = message };
        public static ServiceResult Fail(string message) => new() { Success = false, Message = message };
    }
}
