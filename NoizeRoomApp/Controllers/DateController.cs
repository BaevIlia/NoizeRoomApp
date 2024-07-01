using Microsoft.AspNetCore.Mvc;
using NoizeRoomApp.Models;

namespace NoizeRoomApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DateController : ControllerBase
    {
        List<Booking> bookings = new()
        {
            new Booking {Id = Guid.NewGuid(), BookerName="Петя", Date = DateTime.UtcNow, TimeFrom = DateTime.UtcNow, TimeTo = DateTime.UtcNow.AddHours(2) }
        };


        [HttpGet("booking")]
        public async Task<IActionResult> GetBookingByMonth([FromBody] GetBookingByMonthRequest request)
        {
            return Ok();
        }
    }

    public record GetBookingByMonthRequest(DateTime month);


}
