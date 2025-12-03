using System;

namespace Frock.Contracts
{
    public interface IRouteCreated
    {
        int RouteId { get; }
        int CompanyId { get; }
        string RouteName { get; } // Opcional, si tuvieras nombre
        DateTime CreatedAt { get; }
    }
}