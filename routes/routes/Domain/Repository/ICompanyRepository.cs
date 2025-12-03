using Frock_backend.routes.Domain.Model.Entities;
using Frock_backend.shared.Domain.Repositories; // Asegúrate que este using sea correcto según tu proyecto shared

namespace Frock_backend.routes.Domain.Repository;

public interface ICompanyRepository : IBaseRepository<Company>
{
    // Aquí podrías agregar métodos extra si los necesitas, como FindByName
    Task<Company?> FindByIdAsync(int id);
}