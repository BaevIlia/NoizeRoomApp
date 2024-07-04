namespace NoizeRoomApp.Contracts.BookingContracts
{
    public record UpdateBookRequest(string id, DateTime date, string bookerId, DateTime timeFrom, DateTime timeTo);
}
