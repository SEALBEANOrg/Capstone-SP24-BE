namespace Services.Interfaces.User
{
    public interface IClaimsService
    {
        public Guid GetCurrentUser { get; }
    }
}
