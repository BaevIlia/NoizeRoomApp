namespace NoizeRoomApp.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public Guid AccessToken { get; set; }
        public string NotifyType { get; set; }
    }
}
