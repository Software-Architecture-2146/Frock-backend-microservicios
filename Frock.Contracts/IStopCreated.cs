using System;

namespace Frock.Contracts
{
    public interface IStopCreated
    {
        int Id { get; }
        string Name { get; }
        string Address { get; }
    }
}