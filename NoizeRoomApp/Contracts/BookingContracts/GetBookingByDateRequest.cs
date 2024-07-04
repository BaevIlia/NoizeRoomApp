namespace NoizeRoomApp.Contracts.BookingContracts
{
    public record GetBookingByDateRequest(DateTime dateFrom, DateTime dateTo);
}
