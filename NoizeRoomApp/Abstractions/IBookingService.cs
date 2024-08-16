using NoizeRoomApp.Database.Models;
using NoizeRoomApp.Dtos;

namespace NoizeRoomApp.Abstractions
{
    public interface IBookingService
    {
        Task<List<BookingDto>> GetBookingsByPeriod(DateTime dateFrom, DateTime dateTo);

        Task<List<StatisticDto>> GenerateMonthStatistic(DateTime date);
        Task<bool> MakeBook(BookingEntity book);
        Task<bool> DeleteBook(Guid bookId);
        Task<List<BookingDto>> GetBookingPerDay(Guid userId, DateTime day);
        Task<bool> UpdateBook(BookingDto bookingUpdateData);
        Task<List<DateTime>> GenerateDatesByPeriod(DateTime dateFrom, DateTime dateTo);
        Task<List<DateTime>> GenerateDatesByMonth(DateTime day);
    }
}
