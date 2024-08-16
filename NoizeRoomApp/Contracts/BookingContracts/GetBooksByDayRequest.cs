namespace NoizeRoomApp.Contracts.BookingContracts
{
    public record GetBooksByDayRequest(Guid userId, DateTime date);
}
