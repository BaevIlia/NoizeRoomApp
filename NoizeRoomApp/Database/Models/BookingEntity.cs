namespace NoizeRoomApp.Database.Models
{
    public class BookingEntity
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public Guid BookerId { get; set; }

        public string BookerName { get; set; }
        public DateTime TimeFrom { get; set; }
        public DateTime TimeTo { get; set; }

        public UserEntity? User { get; set; }
    }
}
