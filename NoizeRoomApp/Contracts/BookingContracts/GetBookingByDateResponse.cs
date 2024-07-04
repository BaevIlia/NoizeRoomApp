namespace NoizeRoomApp.Contracts.BookingContracts
{
    public record GetBookingByDateResponse(DateTime date, bool isBooked);
}
