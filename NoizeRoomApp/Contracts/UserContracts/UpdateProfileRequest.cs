namespace NoizeRoomApp.Contracts.UserContracts
{
    public record UpdateProfileRequest(string id, string name, string email, string phoneNumber, int notifyTypeId);
}
