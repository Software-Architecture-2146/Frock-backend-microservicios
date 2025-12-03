using Frock_backend.transport_Company.Domain.Model.Aggregates;
using Frock_backend.transport_Company.Domain.Model.Queries;

namespace Frock_backend.transport_Company.Domain.Services
{
    public interface ICompanyQueryService
    {
        Task<IEnumerable<Companies>> Handle(GetAllCompaniesQuery query);

        Task<Companies?> Handle(GetCompanyByIdQuery query);

        Task<Companies?> Handle(GetCompanyByNameQuery query);

        Task<Companies?> Handle(GetCompanyByFkIdUserQuery query);
        
    }
}
