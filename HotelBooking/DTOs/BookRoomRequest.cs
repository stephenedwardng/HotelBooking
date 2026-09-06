namespace HotelBooking.DTOs
{
    /// <summary>
    /// Data transfer obj for request to book room
    /// </summary>
    public class BookRoomRequest
    {
        public int HotelId { get; set; }
        public int RoomId { get; set; }
        public int GuestId { get; set; }
        public int RoomTypeId { get; set; }
        public int NoOfGuests { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
