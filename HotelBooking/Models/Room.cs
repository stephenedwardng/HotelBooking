namespace HotelBooking.Models
{
    /// <summary>
    /// Model for room
    /// </summary>
    public class Room
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public int RoomTypeId { get; set; }
        public Hotel? Hotel { get; set; }
        public RoomType? RoomType { get; set; }
    }
}
