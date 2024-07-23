namespace NoizeRoomApp.Dtos
{

        public class UserDto() 
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string NotifyType { get; set; } = string.Empty;
        }


    
}
