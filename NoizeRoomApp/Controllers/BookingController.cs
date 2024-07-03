using Microsoft.AspNetCore.Mvc;
using NoizeRoomApp.Models;

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


        List<User> testUsers = new()
        {
            new User { Id = Guid.Parse("fe442bb5-514a-4c27-9b26-f6219866035b"), Name = "Вовчик", RoleId =2}
        };

        [HttpGet("all")]
        public async Task<IActionResult> GetBookingByMonth([FromBody] GetBookingByMonthRequest request)
        {
            List<Booking> bookingsPerMonth = bookings.Where(b => b.Date.Month.Equals(request.month.Month)).ToList();
            return Ok(new GetBookingByMonthResponce(DateTime.UtcNow, bookingsPerMonth));
        }

        #region Statistic
        /*[HttpGet("statistic")]
         public async Task<IActionResult> GetStatisticByMonth([FromBody] GetBookingByMonthRequest request)
         {
             Dictionary<int,int> days = new();

             DateTime firstDay = new DateTime(2024, 7, 1);

             for (DateTime i = firstDay; i<=new DateTime(2024, 7, DateTime.DaysInMonth(2024,7)); i.AddDays(1)) 
             {

                 days.Add(i.Day, CountBooking(i));
             }



              var data = from book in bookings
                         let count = bookings.Count(b => b.Date.Month.Equals(request.month.Month))
                         group book by book.Date into dateGroup
                         orderby dateGroup.Key
                         select new
                         {
                             Date = 
                             Count = dateGroup.Count()

                         };

             List<Statistic> statistic = new();
             foreach (var day in days) 
             {
                 statistic.Add(new Statistic(day.Key, day.Value));
             }

             return Ok(statistic);
         }*/
        #endregion

        #region CountBooking
        /*  int CountBooking(DateTime day) 
          {
              if (bookings.Exists(d => d.Date.Equals(day)))
              {
                  return bookings.Where(b => b.Date.Equals(day)).Count();
              }
              else 
              {
                  return 0;
              }
          }*/

        #endregion

        [HttpPost("book")]
        public async Task<IActionResult> Book([FromBody] PostBookRequest request)
        {
            try
            {
                Booking newBook = new()
                {
                    Id = Guid.NewGuid(),
                    BookerName = request.bookerName,
                    Date = request.date,
                    TimeFrom = request.timeFrom,
                    TimeTo = request.timeTo,
                };
                return Ok(newBook);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }

        [HttpDelete("deleteBook")]
        public async Task<IActionResult> BookDelete([FromBody] string id)
        {
            return Ok();
        }

        [HttpGet("getDayBooking")]
        public async Task<IActionResult> GetDayBooking([FromBody] GetDayByBookRequest request)
        {
            User currentUser = testUsers.Where(u=>u.Id.Equals(Guid.Parse(request.userId))).FirstOrDefault();
            if (currentUser.RoleId == 1)
            {
                return Ok(bookings.Where(u => u.Date.Day.Equals(request.date.Day)));
            }
            else if (currentUser.RoleId == 2)
            {
                return Ok(bookings.Where(u => u.Date.Day.Equals(request.date.Day) && u.BookerId.Equals(Guid.Parse(request.userId))));
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
        /* [HttpGet("date")]
        public async Task<IActionResult> GetDate() 
         {
             return Ok(DateTime.Now);
         }*/



    }

    public record GetBookingByMonthResponce(DateTime date, List<Booking> bookingsList);
    public record GetBookingByMonthRequest(DateTime month);

    public record PostBookRequest(DateTime date, DateTime timeFrom, DateTime timeTo, string bookerName);
    public record Statistic(int dayOfBooking, int count);
    public record GetDayByBookRequest(string userId, DateTime date);
    public record Interval(int from, int to);
}
