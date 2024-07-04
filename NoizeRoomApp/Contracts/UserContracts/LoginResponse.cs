namespace NoizeRoomApp.Contracts.UserContracts
{
    public record LoginResponse(Guid userId, Guid accessToken);
}
