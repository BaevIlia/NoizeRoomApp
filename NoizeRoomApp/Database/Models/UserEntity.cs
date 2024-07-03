namespace NoizeRoomApp.Database.Models
{
    public class UserEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public Guid AccessToken { get; set; }
        public int NotifyTypeId { get; set; }
        public int RoleId { get; set; }

        public List<BookingEntity>? Bookings { get; set; }

        public RoleEntity? Role { get; set; }

        public NotifyEntity? Notify { get; set; }
    }
}
