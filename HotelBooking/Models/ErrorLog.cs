namespace HotelBooking.Models
{
    /// <summary>
    /// Model for error log
    /// </summary>
    public class ErrorLog
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string StackTrace { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
