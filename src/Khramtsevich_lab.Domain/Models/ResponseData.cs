namespace Khramtsevich_lab.Domain.Models
{
    public class ResponseData<T>
    {
        public bool Success { get; set; } = true;
        public string? ErrorMessage { get; set; }
        public T? Data { get; set; }
    }
}
