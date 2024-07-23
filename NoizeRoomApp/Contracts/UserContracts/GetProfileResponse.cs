namespace NoizeRoomApp.Contracts.UserContracts
{
    public record GetProfileResponse(Guid id, string name, string email, string phone, string notifyType);
}
