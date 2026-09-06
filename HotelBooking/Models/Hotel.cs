namespace HotelBooking.Models
{
    /// <summary>
    /// Model for hotel
    /// </summary>
    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int NoOfRooms { get; set; }
    }
}
