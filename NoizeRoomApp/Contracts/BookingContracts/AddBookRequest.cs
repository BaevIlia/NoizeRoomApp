namespace NoizeRoomApp.Contracts.BookingContracts
{
    public record AddBookRequest(DateTime date, DateTime timeFrom, DateTime timeTo, string bookerName, string bookerId);
}
