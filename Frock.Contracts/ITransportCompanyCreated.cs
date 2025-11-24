using System;

namespace Frock.Contracts
{
    public interface ITransportCompanyCreated
    {
        Guid Id { get; }
        string Name { get; }
        DateTime CreatedAt { get; }
    }
}