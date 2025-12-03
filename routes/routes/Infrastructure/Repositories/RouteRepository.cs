using Frock_backend.routes.Domain.Model.Aggregates;
using Frock_backend.routes.Domain.Repository;
using Frock_backend.shared.Infrastructure.Persistences.EFC.Configuration;
using Frock_backend.shared.Infrastructure.Persistences.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Frock_backend.routes.Infrastructure.Repositories
{
    public class RouteRepository(AppDbContext context) : BaseRepository<RouteAggregate>(context), IRouteRepository
    {
        public async Task<List<RouteAggregate>> FindByCompanyId(int companyId)
        {
            // Ahora buscamos directamente en NUESTRA base de datos local
            // porque ya tenemos los datos replicados.
            return await Context.Set<RouteAggregate>()
                .Include(r => r.Stops)
                .Include(r => r.Schedules)
                // IMPORTANTE: Asegúrate de que tu RouteAggregate tenga la propiedad 'CompanyId' o 'FkCompanyId'
                .Where(r => r.CompanyId == companyId) 
                .ToListAsync();
        }
        public Task<List<RouteAggregate>> FindByDistrictId(int districtId)
        {
            throw new NotImplementedException("Obtener los IDs de Stop de distrito vía integración, y luego filtrar.");
        }

        public Task<List<RouteAggregate>> ListRoutes()
        {
            return Context.Set<RouteAggregate>()
                .Include(r => r.Stops)
                .Include(r => r.Schedules)
                .ToListAsync();
        }

        public Task<RouteAggregate?> FindByRouteId(int id)
        {
            return Context.Set<RouteAggregate>()
                .Include(r => r.Stops)
                .Include(r => r.Schedules)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}