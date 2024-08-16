namespace NoizeRoomApp.Dtos
{
    public class IsBookedDto
    {
        DateTime Date { get; set; }
        bool IsBooked { get; set; }

        public IsBookedDto(DateTime date, bool isBooked)
        {
            Date = date;
            IsBooked = isBooked;
        }
    }
}
