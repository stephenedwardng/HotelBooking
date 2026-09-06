using HotelBooking.Data;
using HotelBooking.Exceptions;
using HotelBooking.Models;

namespace HotelBooking.Middleware
{
    /// <summary>
    /// Middleware for logging errors to db and returning JSON error responses
    /// </summary>
    public class ErrorLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorLoggingMiddleware> _logger;

        public ErrorLoggingMiddleware(RequestDelegate next, ILogger<ErrorLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware to handle exceptions and log to db
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <param name="db">DB context</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context, HotelBookingDbContext db)
        {
            try
            {
                await _next(context);
            }
            catch (BusinessRuleException ex)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
                return;
            }
            catch (Exception ex)
            {
                // Log to console/file
                _logger.LogError(ex, "Unhandled exception");

                // Log to database
                var log = new ErrorLog
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace ?? "",
                    CreatedAt = DateTime.UtcNow
                };

                db.ErrorLogs.Add(log);
                await db.SaveChangesAsync();

                // Return clean JSON error
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var error = new
                {
                    message = "An unexpected error occurred.",
                    reference = log.Id
                };

                await context.Response.WriteAsJsonAsync(error);
            }
        }

    }
}
