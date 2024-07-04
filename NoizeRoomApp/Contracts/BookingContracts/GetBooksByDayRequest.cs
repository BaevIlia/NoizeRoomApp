namespace NoizeRoomApp.Contracts.BookingContracts
{
    public record GetBooksByDayRequest(string userId, DateTime date);
}
