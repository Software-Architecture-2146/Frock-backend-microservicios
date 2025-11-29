namespace Frock.Contracts
{
    public interface IUserCreated
    {
        int Id { get; }
        string Username { get; }
        string Email { get; }
        string Role { get; } // Opcional: Puede ser útil que otros sepan si es Admin
    }
}