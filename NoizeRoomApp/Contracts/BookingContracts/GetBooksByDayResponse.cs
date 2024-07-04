namespace NoizeRoomApp.Contracts.BookingContracts
{
    public record GetBooksByDayResponse(DateTime dateBooking, DateTime timeFrom, DateTime timeTo, string bookerName);
}
