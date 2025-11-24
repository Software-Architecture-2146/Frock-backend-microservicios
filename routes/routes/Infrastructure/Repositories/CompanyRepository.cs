using Frock_backend.routes.Domain.Model.Entities;
using Frock_backend.routes.Domain.Repository;
using Frock_backend.shared.Infrastructure.Persistences.EFC.Configuration;
using Frock_backend.shared.Infrastructure.Persistences.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Frock_backend.routes.Infrastructure.Repositories;

public class CompanyRepository : BaseRepository<Company>, ICompanyRepository
{
    public CompanyRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Company?> FindByIdAsync(Guid id)
    {
        return await Context.Set<Company>().FirstOrDefaultAsync(c => c.Id == id);
    }
}