namespace HotelBooking.Models
{
    /// <summary>
    /// Model for Room Type
    /// </summary>
    public class RoomType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }

    }
}
