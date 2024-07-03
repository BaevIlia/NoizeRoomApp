namespace NoizeRoomApp.Models
{
    public class Booking
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }

        public string BookerName { get; set; }
        public DateTime TimeFrom { get; set; }
        public DateTime TimeTo { get; set; }

        public Guid BookerId { get; set; }
    }


}
