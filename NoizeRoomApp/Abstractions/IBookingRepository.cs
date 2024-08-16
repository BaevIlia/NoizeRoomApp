using NoizeRoomApp.Database.Models;
using NoizeRoomApp.Dtos;

namespace NoizeRoomApp.Abstractions
{
    public interface IBookingRepository
    {
        BookingEntity GetBooking(Guid id);
        List<IsBookedDto> GetBookingsByDate(List<DateTime> dates);
        List<StatisticDto> GetStatistic(List<DateTime> dates);
        bool MakeBooking(string bookerName, BookingEntity newBooking);
        string GetBookerName(Guid id);

        bool DeleteBooking(Guid id);
        bool UpdateBooking(BookingEntity newBooking);

        List<GetBooksByDayDto> GetBooksByDay(Guid userId, DateTime date);
    }
}
