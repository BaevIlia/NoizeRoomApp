namespace NoizeRoomApp.Dtos
{
    public class BookingDto
    {
       public DateTime Date { get; set; }
       public DateTime TimeFrom { get; set; }
       public DateTime TimeTo { get; set; }
       public string BookerName { get; set; }

        public BookingDto(DateTime date, DateTime timeFrom, DateTime timeTo, string bookerName)
        {
            Date = date;
            TimeFrom = timeFrom;
            TimeTo = timeTo;
            BookerName = bookerName;
        }
    }
}
