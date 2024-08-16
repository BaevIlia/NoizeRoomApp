namespace NoizeRoomApp.Dtos
{
    public class StatisticDto
    {
        DateTime Date { get; set; }
        int Count { get; set; }

        public StatisticDto(DateTime date, int count)
        {
            Date = date;
            Count = count;
        }
    }
}
