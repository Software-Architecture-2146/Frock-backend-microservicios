using Frock_backend.routes.Domain.Model.Entities;
using Frock_backend.shared.Domain.Repositories;

namespace Frock_backend.routes.Domain.Repository
{
    public interface IStopRepository : IBaseRepository<Stop>
    {
        Task<bool> ExistsAsync(int id); // Para validar rápido
    }
}