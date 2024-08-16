using NoizeRoomApp.Database.Models;
using NoizeRoomApp.Dtos;

namespace NoizeRoomApp.Abstractions
{
    public interface IBookingService
    {
        List<IsBookedDto> GetBookingsByPeriod(DateTime dateFrom, DateTime dateTo);

        List<StatisticDto> GenerateMonthStatistic(DateTime date);
        Task<bool> MakeBook(Guid bookerId, BookingEntity book);
        Task<bool> DeleteBook(Guid bookId);
        Task<List<BookingDto>> GetBookingPerDay(Guid userId, DateTime day);
        Task<bool> UpdateBook(Guid bookingId, BookingDto bookingUpdateData);
        List<DateTime> GenerateDatesByPeriod(DateTime dateFrom, DateTime dateTo);
        List<DateTime> GenerateDatesByMonth(DateTime day);
    }
}
