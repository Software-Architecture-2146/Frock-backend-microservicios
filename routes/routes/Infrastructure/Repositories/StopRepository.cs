using Frock_backend.routes.Domain.Model.Entities;
using Frock_backend.routes.Domain.Repository;
using Frock_backend.shared.Infrastructure.Persistences.EFC.Configuration;
using Frock_backend.shared.Infrastructure.Persistences.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Frock_backend.routes.Infrastructure.Repositories
{
    public class StopRepository(AppDbContext context) : BaseRepository<Stop>(context), IStopRepository
    {
        public async Task<bool> ExistsAsync(int id)
        {
            return await Context.Set<Stop>().AnyAsync(s => s.Id == id);
        }
    }
}