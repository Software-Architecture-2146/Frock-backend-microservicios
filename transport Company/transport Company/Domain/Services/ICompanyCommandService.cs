using Frock_backend.transport_Company.Domain.Model.Aggregates;
using Frock_backend.transport_Company.Domain.Model.Commands;

namespace Frock_backend.transport_Company.Domain.Services
{
    public interface ICompanyCommandService
    {
        Task<Companies?> Handle(CreateCompanyCommand command);
        Task<Companies?> Handle(UpdateCompanyCommand command);
        Task<Companies?> Handle(DeleteCompanyCommand command);
    }
}
