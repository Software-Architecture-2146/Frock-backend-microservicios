using Frock_backend.stops.Domain.Model.Entities;
using Frock_backend.stops.Domain.Repositories;
using Frock_backend.shared.Infrastructure.Persistences.EFC.Configuration;
using Frock_backend.shared.Infrastructure.Persistences.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Frock_backend.stops.Infrastructure.Repositories
{
    public class CompanyRepository(AppDbContext context) : BaseRepository<Company>(context), ICompanyRepository
    {
        public async Task<bool> ExistsAsync(int id)
        {
            return await Context.Set<Company>().AnyAsync(c => c.Id == id);
        }
    }
}