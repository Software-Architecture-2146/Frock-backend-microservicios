using Frock_backend.shared.Domain.Repositories;
using Frock_backend.shared.Infrastructure.Persistences.EFC.Configuration;
using Frock_backend.shared.Infrastructure.Persistences.EFC.Repositories;


using Frock_backend.transport_Company.Domain.Model.Aggregates;
using Frock_backend.transport_Company.Domain.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Frock_backend.transport_Company.Infrastructure.Repositories
{
    public class CompanyRepository(AppDbContext context) : BaseRepository<Companies>(context), ICompanyRepository
    {
  
        public async Task<Companies?> FindByNameAsync(string name)
        {
            return await context.Set<Companies>()
                .FirstOrDefaultAsync(f => f.Name == name);
        }
        
        public async Task<Companies?> FindByFkIdUserAsync(int fkIdUser)
        {
            return await context.Set<Companies>().FirstOrDefaultAsync(c => c.FkIdUser == fkIdUser);
        }        
    
    }
}
