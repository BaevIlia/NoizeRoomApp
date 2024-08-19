using Microsoft.AspNetCore.Mvc;
using NoizeRoomApp.Database;
using NoizeRoomApp.Database.Models;
using NoizeRoomApp.Contracts.BookingContracts;
using NoizeRoomApp.Abstractions;
using NoizeRoomApp.Dtos;
using CSharpFunctionalExtensions;

namespace NoizeRoomApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {


        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService) 
        {
            _bookingService = bookingService;
        }
       
        /// <summary>
        /// Метод получения броней на определённый промежуток данных. На вход принимает дату "От" и дату "До"
        /// </summary>
        /// <param name="request">Возвращается коллекцию вида <DateTime date, bool isBooking></param>
        /// <returns></returns>
        [HttpPost("getBookingsByDate")]
        public async Task<IActionResult> GetBookingByDate([FromBody] GetBookingByDateRequest request)
        {
            var result = _bookingService.GetBookingsByPeriod(request.dateFrom, request.dateTo);

            if (result.Count != 0)
                return Ok(result);
            /*
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


            return Ok(responce);*/
            return BadRequest();
        }

        /// <summary>
        /// Запрос на получение статистики по посещениям за месяц. На вход принимает дату из которой выбирается месяц
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Возвращает коллекцию вида <DateTime date, int count></returns>
        [HttpPost("getStatistic")]
         public async Task<IActionResult> GetStatisticByMonth([FromBody] GetStatisticRequest request)
        {
            List<StatisticDto> result = _bookingService.GenerateMonthStatistic(request.date);

            if (result.Count != 0)
                return Ok(result);
            else
                return BadRequest();
            /*
            //Генерация дат месяца
            List<DateTime> dates = GenerateDatesByMonth(request.date);
            //Генерация статистики
            List<GetStatisticResponse> responce = GenerateStatistic(dates);

            return Ok(responce);*/
    
        }




        /// <summary>
        /// Запрос на создание брони. 
        /// </summary>
        /// <param name="request">Принимает идентификатор пользователя, дату бронирования, время "От" и время "До"</param>
        /// <returns></returns>
        [HttpPost("book")]
        public async Task<IActionResult> Book([FromBody] AddBookRequest request)
        {
            BookingEntity booking = new()
            {
                BookerId = Guid.Parse(request.bookerId),
                Date = request.date,
                TimeFrom = request.timeFrom,
                TimeTo = request.timeTo,

            };
            var result = await _bookingService.MakeBook(Guid.Parse(request.bookerId), booking);
            if (result)
                return Ok();
            else
                return BadRequest();
        }
        /// <summary>
        /// Запрос на удаление брони. 
        /// </summary>
        /// <param name="request">Принимает идентификатор брони</param>
        /// <returns></returns>
        [HttpDelete("deleteBook")]
        public async Task<IActionResult> BookDelete([FromBody] DeleteBookRequest request)
        {
            var result = await _bookingService.DeleteBook(Guid.Parse(request.id));

            if (result)
                return Ok();

            /*
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
            */
            return BadRequest();
        }

        /// <summary>
        /// Запрос на получение броней на день. 
        /// </summary>
        /// <param name="request">Принимает дату и идентификатор текущего пользователя</param>
        /// <returns>Коллекция объектов вида <DateTime date, DateTime timeFrom, DateTime timeTo, string bookerName></returns>
        [HttpPost("getDayBooking")]
        public async Task<IActionResult> GetDayBooking([FromBody] GetBooksByDayRequest request)
        {
            var result = await _bookingService.GetBookingPerDay(request.userId, request.date);

            if (result.Count != 0)
                return Ok(result);
            else
                return NoContent();

          
        }

        /// <summary>
        /// Запрос на редактирование брони. На вход принимает идентификатор пользователя, дату бронирования, время "От" и время "До"
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("bookUpdate")]
        public async Task<IActionResult> Update([FromBody] UpdateBookRequest request)
        {
            string bookerName = await _bookingService.GetBookerName(Guid.Parse(request.bookerId));
            BookingDto bookingDataForUpdate = new(request.date, request.timeFrom, request.timeTo, bookerName);


            var result = await _bookingService.UpdateBook(Guid.Parse(request.id), bookingDataForUpdate);

            

            if (result)
                return Ok(result);
            else
                return BadRequest();
        }
   
        /// <summary>
        /// Метод генерации данных по интервалу "От" и "До"
        /// </summary>
        /// <param name="dateFrom">Дата "От"</param>
        /// <param name="dateTo">Дата "До"</param>
        /// <returns></returns>
   
        

    }

    
    

  




    

   

}
