using NoizeRoomApp.Abstractions;
using NoizeRoomApp.Database.Models;
using NoizeRoomApp.Dtos;

namespace NoizeRoomApp.Services
{
    public class BookingService : IBookingService
    {

        private readonly IBookingRepository _repository;
        public BookingService(IBookingRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> DeleteBook(Guid bookId)
        {
            var result = await _repository.DeleteBooking(bookId);

            return result;
        }

        public List<DateTime> GenerateDatesByMonth(DateTime date)
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

        public List<DateTime> GenerateDatesByPeriod(DateTime dateFrom, DateTime dateTo)
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

        public List<StatisticDto> GenerateMonthStatistic(DateTime date)
        {
            List<DateTime> dates = GenerateDatesByMonth(date);
            List<StatisticDto> statistic = new();

            statistic = _repository.GetStatistic(dates);

            return statistic;

        }

        public async Task<List<BookingDto>> GetBookingPerDay(Guid userId, DateTime day)
        {
            var result = await _repository.GetBooksByDay(userId, day);

            if (result is not null && result.GetType() == typeof(List<BookingDto>))
                return result;
            else
                return new List<BookingDto>();
        }

        public List<IsBookedDto> GetBookingsByPeriod(DateTime dateFrom, DateTime dateTo)
        {
            List<DateTime> dates = GenerateDatesByPeriod(dateFrom, dateTo);
            List<BookingDto> bookings = new();
            var result = _repository.GetBookingsByDate(dates);
            if (result.Count == 0 && result.GetType() == typeof(IsBookedDto))
                return result;
            else
                return new List<IsBookedDto>();

        }

        public async Task<bool> MakeBook(Guid bookerId, BookingEntity book)
        {
            var result = await _repository.CreateBook(bookerId, book);

            return result;
        }

        public async Task<bool> UpdateBook(Guid bookingId, BookingDto bookingUpdateData)
        {
            var result = await _repository.UpdateBooking(bookingId, bookingUpdateData);
            return result;
        }
        public async Task<string> GetBookerName(Guid bookerId) 
        {
            var result = await _repository.GetBookerName(bookerId);
            return result;
        }
    }
}
