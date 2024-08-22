namespace NoizeRoomApp.Dtos
{
    public class StatisticDto
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }

        public StatisticDto(DateTime date, int count)
        {
            Date = date;
            Count = count;
        }
    }
}
