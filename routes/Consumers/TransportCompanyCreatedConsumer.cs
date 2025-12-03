using MassTransit;
using Frock.Contracts;
// Asegúrate de que estos namespace coincidan con los que creaste
using Frock_backend.routes.Domain.Repository;
using Frock_backend.routes.Domain.Model.Entities;
using Frock_backend.shared.Domain.Repositories;

namespace Frock_backend.routes.Consumers; 

public class TransportCompanyCreatedConsumer : IConsumer<ITransportCompanyCreated>
{
    // 1. Declaramos las herramientas que vamos a usar (Repositorio y UnitOfWork)
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    // 2. Las pedimos en el constructor (Inyección de Dependencias)
    public TransportCompanyCreatedConsumer(ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    // 3. Este método se ejecuta cuando llega el mensaje
    public async Task Consume(ConsumeContext<ITransportCompanyCreated> context)
    {
        var message = context.Message;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[RABBIT MQ - EVENTO RECIBIDO] 📨");
        Console.WriteLine($"--> Procesando empresa: {message.Name}");
        Console.ResetColor();

        try
        {
            // A. Verificamos si ya existe la empresa en Routes (para no duplicar)
            var existingCompany = await _companyRepository.FindByIdAsync(message.Id);
            if (existingCompany != null)
            {
                Console.WriteLine("   [INFO] La empresa ya existía en la BD de Routes. No hago nada.");
                return;
            }

            // B. Creamos la nueva entidad 'Company' local para Routes
            var newCompany = new Company
            {
                Id = message.Id,
                Name = message.Name,
                LastUpdated = DateTime.UtcNow,
                UserId = message.UserId
            };

            // C. Guardamos en la base de datos
            await _companyRepository.AddAsync(newCompany);
            await _unitOfWork.CompleteAsync();

            Console.WriteLine($"   [SUCCESS] ✅ Empresa '{message.Name}' guardada correctamente en la BD de ROUTES.");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"   [ERROR] No se pudo guardar en BD: {ex.Message}");
            Console.ResetColor();
        }
    }
}