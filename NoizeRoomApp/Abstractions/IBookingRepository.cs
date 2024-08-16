using NoizeRoomApp.Database.Models;
using NoizeRoomApp.Dtos;

namespace NoizeRoomApp.Abstractions
{
    public interface IBookingRepository
    {
        Task<BookingEntity> GetBooking(Guid id);
        List<IsBookedDto> GetBookingsByDate(List<DateTime> dates);
        List<StatisticDto> GetStatistic(List<DateTime> dates);
        Task<bool> CreateBook(Guid bookerId, BookingEntity newBooking);
        Task<string> GetBookerName(Guid id);

        Task<bool> DeleteBooking(Guid id);
        Task<bool> UpdateBooking(Guid bookingId, BookingDto newBooking);

        Task<List<BookingDto>> GetBooksByDay(Guid userId, DateTime date);
    }
}
