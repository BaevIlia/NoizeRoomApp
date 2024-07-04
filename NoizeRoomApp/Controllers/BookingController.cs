using Microsoft.AspNetCore.Mvc;
using NoizeRoomApp.Database;
using NoizeRoomApp.Models;
using NoizeRoomApp.Database.Models;

namespace NoizeRoomApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        List<Booking> bookings = new()
        {
            new Booking {Id = Guid.NewGuid(), BookerName="Петя", Date = new DateTime(2024, 7, 3), TimeFrom = DateTime.Now, TimeTo = DateTime.Now.AddHours(2) },
            new Booking {Id = Guid.NewGuid(), BookerName="Вася", Date = new DateTime(2024, 7, 1), TimeFrom = DateTime.Now, TimeTo = DateTime.Now.AddHours(2) },
            new Booking {Id = Guid.NewGuid(), BookerName="Дима", Date = new DateTime(2024, 7, 1), TimeFrom = DateTime.Now, TimeTo = DateTime.Now.AddHours(2) },
            new Booking {Id = Guid.NewGuid(), BookerName="Вовчик", Date = new DateTime(2024, 7, 3), TimeFrom = DateTime.Now, TimeTo = DateTime.Now.AddHours(2), BookerId = Guid.Parse("fe442bb5-514a-4c27-9b26-f6219866035b") }
        };

        private readonly PostgreSQLContext _context;
        public BookingController(PostgreSQLContext context) 
        {
            _context = context;
        }
        List<User> testUsers = new()
        {
            new User { Id = Guid.Parse("fe442bb5-514a-4c27-9b26-f6219866035b"), Name = "Вовчик", RoleId =2}
        };

        [HttpGet("getBookingsByMonth")]
        public async Task<IActionResult> GetBookingByMonth([FromBody] GetBookingByDateRequest request)
        {
            List<DateTime> dates = DatesGeneration(request.dateFrom, request.dateTo);
            List<GetBookingByMonthResponce> responce = new();
            foreach (var date in dates) 
            {
                if (_context.Bookings.Any(b => b.Date.Equals(date)))
                {
                    responce.Add(new GetBookingByMonthResponce(date, true));
                }
                else 
                {
                    responce.Add(new GetBookingByMonthResponce(date, false));
                }
            }


            return Ok(responce);
        }

       
        [HttpGet("statistic")]
         public async Task<IActionResult> GetStatisticByMonth([FromBody] GetStatisticRequest request)
        {
            List<DateTime> dates = GenerateDatesByMonth(request.date);
            List<GetStatisticResponce> responce = GenerateStatistic(dates);

            return Ok(responce);
        }

        



        [HttpPost("book")]
        public async Task<IActionResult> Book([FromBody] PostBookRequest request)
        {
            try
            {
                BookingEntity newBook = new()
                {
                    Id = Guid.NewGuid(),
                    BookerId = Guid.Parse(request.bookerId),
                    BookerName = request.bookerName,
                    Date = request.date,
                    TimeFrom = request.timeFrom,
                    TimeTo = request.timeTo,
                };

                if (newBook is null) 
                {
                    return NoContent();
                }

                _context.Bookings.Add(newBook);
                _context.SaveChanges();
                return Ok(newBook);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }

        [HttpDelete("deleteBook")]
        public async Task<IActionResult> BookDelete([FromBody] DeleteBookRequest request)
        {
            BookingEntity bookForDelete = _context.Bookings.Where(b => b.Id.Equals(Guid.Parse(request.id))).FirstOrDefault();

            if (bookForDelete is null) 
            {
                return NoContent();
            }
            _context.Bookings.Remove(bookForDelete);
            _context.SaveChanges();
            return Ok();
        }

        [HttpGet("getDayBooking")]
        public async Task<IActionResult> GetDayBooking([FromBody] GetBooksByDayRequest request)
        {
            UserEntity currentUser = _context.Users.Where(u=>u.Id.Equals(Guid.Parse(request.userId))).FirstOrDefault();

            List<GetBooksByDayResponse> responce = new();
            if (currentUser.RoleId == 1)
            {
                responce = (from books in _context.Bookings
                           .Where(b => b.Date.Day.Equals(request.date.Day))
                            select new GetBooksByDayResponse(books.Date, books.TimeFrom, books.TimeTo, books.BookerName)).ToList();

                return Ok(responce);
            }
            else if (currentUser.RoleId == 2)
            {
                responce = (from books in _context.Bookings
                            .Where(b => b.Date.Day.Equals(request.date.Day)&& b.BookerId.Equals(currentUser.Id))
                            select new GetBooksByDayResponse(books.Date, books.TimeFrom, books.TimeTo, books.BookerName)).ToList();
                return Ok(responce);
            }
            else 
            {
                return BadRequest();
            }
        }

        [HttpPut("bookChange")]
        public async Task<IActionResult> BookChange([FromBody] Booking request)
        {
            try
            {
                bookings.Add(request);
                return Ok(request);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
   

        private static List<DateTime> DatesGeneration(DateTime dateFrom, DateTime dateTo)
        {
            List<DateTime> dates = new();
            DateTime startDate = dateFrom;
            DateTime nextDate = startDate.AddDays(1);
            int count = 1;
            while (nextDate <= dateTo)
            {
                dates.Add(startDate);
                nextDate = startDate.AddDays(1);
                startDate = nextDate;

            }

            return dates;
        }
        private static List<DateTime> GenerateDatesByMonth(DateTime date)
        {
            List<DateTime> dates = new();

            int count = DateTime.DaysInMonth(date.Year, date.Month);

            for (int i = 1; i <= count; i++)
            {
                dates.Add(new DateTime(date.Year, date.Month, i));
            }

            return dates;
        }
        private List<GetStatisticResponce> GenerateStatistic(List<DateTime> dates)
        {
            List<GetStatisticResponce> responce = new();
            foreach (var date in dates)
            {
                responce.Add(new GetStatisticResponce(date, _context.Bookings.Where(b => b.Date.Equals(date)).Count()));

            }

            return responce;
        }

    }

    public record GetBookingByMonthResponce(DateTime date, bool isBooked);
    public record GetBookingByDateRequest(DateTime dateFrom, DateTime dateTo);

    public record PostBookRequest(DateTime date, DateTime timeFrom, DateTime timeTo, string bookerName, string bookerId);
    public record Statistic(int dayOfBooking, int count);
    public record GetBooksByDayRequest(string userId, DateTime date);
    public record Interval(int from, int to);

    public record DeleteBookRequest(string id);

    public record GetBooksByDayResponse(DateTime dateBooking, DateTime timeFrom, DateTime timeTo, string bookerName);

    public record GetStatisticRequest(DateTime date);
    public record GetStatisticResponce(DateTime date, int count);
}
