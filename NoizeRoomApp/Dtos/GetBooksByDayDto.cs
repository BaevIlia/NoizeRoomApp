namespace NoizeRoomApp.Dtos
{
    public class GetBooksByDayDto
    {
        DateTime Date { get; set; }
        DateTime TimeFrom { get; set; }
        DateTime TimeTo { get; set; }
        string BookerName { get; set; }
    }
}
