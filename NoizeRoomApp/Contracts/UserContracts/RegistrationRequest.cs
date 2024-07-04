namespace NoizeRoomApp.Contracts.UserContracts
{
    public record RegistrationRequest(string email, string name, string phone, string password, int notifyTypeId, int roleId);
}
