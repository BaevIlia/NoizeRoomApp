using Microsoft.AspNetCore.Mvc;
using NoizeRoomApp.Database;
using NoizeRoomApp.Models;
using NoizeRoomApp.Database.Models;
using NoizeRoomApp.Contracts.BookingContracts;

namespace NoizeRoomApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
    

        private readonly PostgreSQLContext _context;
        public BookingController(PostgreSQLContext context) 
        {
            _context = context;
        }
       
        /// <summary>
        /// Метод получения броней на определённый промежуток данных. На вход принимает дату "От" и дату "До"
        /// </summary>
        /// <param name="request">Возвращается коллекцию вида <DateTime date, bool isBooking></param>
        /// <returns></returns>
        [HttpGet("getBookingsByDate")]
        public async Task<IActionResult> GetBookingByDate([FromBody] GetBookingByDateRequest request)
        {
            //Генерируется список дат за период
            List<DateTime> dates = DatesGeneration(request.dateFrom, request.dateTo);

            List<GetBookingByDateResponse> responce = new();
        
            foreach (var date in dates) 
            {
                //Если на данную дату присутствует хоть одна запись, isBooking = true
                if (_context.Bookings.Any(b => b.Date.Equals(date)))
                {
                    responce.Add(new GetBookingByDateResponse(date, true));
                }
                //Если нет, isBooking = false
                else 
                {
                    responce.Add(new GetBookingByDateResponse(date, false));
                }
            }


            return Ok(responce);
        }

        /// <summary>
        /// Запрос на получение статистики по посещениям за месяц. На вход принимает дату из которой выбирается месяц
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Возвращает коллекцию вида <DateTime date, int count></returns>
        [HttpGet("getStatistic")]
         public async Task<IActionResult> GetStatisticByMonth([FromBody] GetStatisticRequest request)
        {
            //Генерация дат месяца
            List<DateTime> dates = GenerateDatesByMonth(request.date);
            //Генерация статистики
            List<GetStatisticResponse> responce = GenerateStatistic(dates);

            return Ok(responce);
        }




        /// <summary>
        /// Запрос на создание брони. 
        /// </summary>
        /// <param name="request">Принимает идентификатор пользователя, дату бронирования, время "От" и время "До"</param>
        /// <returns></returns>
        [HttpPost("book")]
        public async Task<IActionResult> Book([FromBody] AddBookRequest request)
        {
            //Поиск имени бронирующего по его идентификатору
            string bookerName = _context.Users.Where(u => u.Id.Equals(Guid.Parse(request.bookerId))).Select(u=>u.Name).FirstOrDefault();
            try
            {
                //Создание брони
                BookingEntity newBook = new()
                {
                    Id = Guid.NewGuid(),
                    BookerId = Guid.Parse(request.bookerId),
                    BookerName = bookerName,
                    Date = request.date,
                    TimeFrom = request.timeFrom,
                    TimeTo = request.timeTo,
                };

                if (newBook is null) 
                {
                    return NoContent();
                }
                //Добавление и сохранение брони
                _context.Bookings.Add(newBook);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }
        /// <summary>
        /// Запрос на удаление брони. 
        /// </summary>
        /// <param name="request">Принимает идентификатор брони</param>
        /// <returns></returns>
        [HttpDelete("deleteBook")]
        public async Task<IActionResult> BookDelete([FromBody] DeleteBookRequest request)
        {
            //Поиск брони на удаление
            BookingEntity bookForDelete = _context.Bookings.Where(b => b.Id.Equals(Guid.Parse(request.id))).FirstOrDefault();

            if (bookForDelete is null) 
            {
                return NoContent();
            }
            //Удаление и сохранение изменений
            _context.Bookings.Remove(bookForDelete);
            _context.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// Запрос на получение броней на день. 
        /// </summary>
        /// <param name="request">Принимает дату и идентификатор текущего пользователя</param>
        /// <returns>Коллекция объектов вида <DateTime date, DateTime timeFrom, DateTime timeTo, string bookerName></returns>
        [HttpGet("getDayBooking")]
        public async Task<IActionResult> GetDayBooking([FromBody] GetBooksByDayRequest request)
        {
            //Поиск текущего пользователя
            UserEntity currentUser = _context.Users.Where(u=>u.Id.Equals(Guid.Parse(request.userId))).FirstOrDefault();

            List<GetBooksByDayResponse> responce = new();
            //Если роль текущего пользователя "Администратор", то выводится информация по всем броням на день
            if (currentUser.RoleId == 1)
            {
                responce = (from books in _context.Bookings
                           .Where(b => b.Date.Day.Equals(request.date.Day))
                            select new GetBooksByDayResponse(books.Date, books.TimeFrom, books.TimeTo, books.BookerName)).ToList();

                return Ok(responce);
            }
            //Если роль текущего пользователя "Пользователь", то выводится информация по его броням на день
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

        /// <summary>
        /// Запрос на редактирование брони. На вход принимает идентификатор пользователя, дату бронирования, время "От" и время "До"
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("bookUpdate")]
        public async Task<IActionResult> Update([FromBody] UpdateBookRequest request)
        {
            //Поиск брони по идентификатору
            BookingEntity bookForUpdate = _context.Bookings.Where(b=>b.Id.Equals(Guid.Parse(request.id))).FirstOrDefault();

            if (bookForUpdate is null) 
            {
                return NoContent();
            }
            //Запись в БД
            try
            {
                bookForUpdate.Date = request.date;
                bookForUpdate.BookerName = _context.Users.Where(u => u.Id.Equals(Guid.Parse(request.bookerId))).Select(u => u.Name).FirstOrDefault();
                bookForUpdate.TimeFrom = request.timeFrom;
                bookForUpdate.TimeTo = request.timeTo;

                _context.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
   
        /// <summary>
        /// Метод генерации данных по интервалу "От" и "До"
        /// </summary>
        /// <param name="dateFrom">Дата "От"</param>
        /// <param name="dateTo">Дата "До"</param>
        /// <returns></returns>
        private static List<DateTime> DatesGeneration(DateTime dateFrom, DateTime dateTo)
        {

            List<DateTime> dates = new();
            DateTime startDate = dateFrom;
            DateTime nextDate = startDate.AddDays(1);
            //Генерация происходит путём добавления по одному дню к дате, пока очередная дата для добавления не будет позже чем дата "До"
            while (nextDate <= dateTo)
            {
                dates.Add(startDate);
                nextDate = startDate.AddDays(1);
                startDate = nextDate;

            }

            return dates;
        }

        /// <summary>
        /// Генерация дат на определённый месяц
        /// </summary>
        /// <param name="date">Дата из которой метод берёт месяц и год</param>
        /// <returns></returns>
        private static List<DateTime> GenerateDatesByMonth(DateTime date)
        {
            List<DateTime> dates = new();

            //Через метод DaysInMonth получается количество дней этого месяца
            int count = DateTime.DaysInMonth(date.Year, date.Month);

            //Заполнение списка дат
            for (int i = 1; i <= count; i++)
            {
                dates.Add(new DateTime(date.Year, date.Month, i));
            }

            return dates;
        }

        /// <summary>
        /// Метод генерации статистики посещений за месяц
        /// </summary>
        /// <param name="dates">Список сгенерированных дат за месяц</param>
        /// <returns></returns>
        private List<GetStatisticResponse> GenerateStatistic(List<DateTime> dates)
        {
            List<GetStatisticResponse> responce = new();
            //По каждой дате проверяется количество записей о брони
            foreach (var date in dates)
            {
                responce.Add(new GetStatisticResponse(date, _context.Bookings.Where(b => b.Date.Equals(date)).Count()));

            }

            return responce;
        }

    }

    
    

  




    

   

}
