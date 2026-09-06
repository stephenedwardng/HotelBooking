namespace HotelBooking.Exceptions
{
    /// <summary>
    /// Custom exception for business rules
    /// </summary>
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message) { }
    }
}
