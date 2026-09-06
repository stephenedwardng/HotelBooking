namespace HotelBooking.Models
{
    /// <summary>
    /// Model for booking made by a guest for a specific room within a hotel, including the booking period and number of guests.
    /// </summary>
    public class Booking
    {
        public int Id { get; set; }
        public int GuestId { get; set; }
        public Guest? Guest { get; set; } 
        public int RoomId { get; set; }
        public Room? Room { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NoOfGuests { get; set; }
        public string Reference { get; set; } = Guid.NewGuid().ToString();
    }
}
