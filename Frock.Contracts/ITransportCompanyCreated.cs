using System;

namespace Frock.Contracts
{
    public interface ITransportCompanyCreated
    {
        int Id { get; }
        string Name { get; }
        DateTime CreatedAt { get; }
        int UserId { get; }
    }
}