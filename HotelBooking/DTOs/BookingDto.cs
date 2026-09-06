namespace HotelBooking.DTOs
{
    public class BookingDto
    {
        public string Reference { get; set; }
        public int RoomId { get; set; }
        public int GuestId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NoOfGuests { get; set; }
    }
}
