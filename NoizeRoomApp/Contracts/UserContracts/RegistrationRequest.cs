namespace NoizeRoomApp.Contracts.UserContracts
{
    public record RegistrationRequest(string email, string name, string phone, string password, string notifyType, string role);
}
