using Frock_backend.stops.Domain.Model.Entities;
using Frock_backend.shared.Domain.Repositories;

namespace Frock_backend.stops.Domain.Repositories
{
    public interface ICompanyRepository : IBaseRepository<Company>
    {
        Task<bool> ExistsAsync(int id);
    }
}