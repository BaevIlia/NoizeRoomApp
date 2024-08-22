namespace NoizeRoomApp.Dtos
{
    public class IsBookedDto
    {
        public DateTime Date { get; set; }
        public bool IsBooked { get; set; }

        public IsBookedDto(DateTime date, bool isBooked)
        {
            Date = date;
            IsBooked = isBooked;
        }
    }
}
