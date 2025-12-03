using Frock_backend.routes.Domain.Model.Aggregates;
using Frock_backend.routes.Domain.Model.Commands;
using Frock_backend.routes.Domain.Repository;
using Frock_backend.routes.Domain.Service;
using Frock_backend.shared.Domain.Repositories;
using MassTransit;
using Frock.Contracts;

namespace Frock_backend.routes.Application.Internal.CommandServices
{
    public class RouteCommandService(
        IRouteRepository routeRepository,
        ICompanyRepository companyRepository,
        IStopRepository stopRepository,
        IUnitOfWork unitOfWork,
        IPublishEndpoint publishEndpoint) : IRouteCommandService
    {
        public async Task<RouteAggregate?> Handle(CreateFullRouteCommand command)
        {
            var companyExists = await companyRepository.FindByIdAsync(command.CompanyId);
            if (companyExists == null)
            {
                throw new Exception($"La empresa con ID {command.CompanyId} no existe o no ha sido sincronizada en Routes.");
            }
            if (companyExists.UserId != command.UserId)
            {
                // ¡ALERTA DE SEGURIDAD!
                throw new UnauthorizedAccessException("¡No puedes crear rutas para una empresa que no es tuya!");
            }
            // 2. VALIDAR QUE LAS PARADAS EXISTAN (LOCALMENTE)
            foreach (var stopId in command.StopsIds)
            {
                if (!await stopRepository.ExistsAsync(stopId))
                {
                    throw new Exception($"La parada con ID {stopId} no existe en la base de datos de Routes.");
                }
            }
            
            var newRoute = new RouteAggregate(
                command.CompanyId,
                command.Price,
                command.Duration,
                command.Frequency);
            foreach (var stopId in command.StopsIds)
            {
                // Asumiendo que tienes un método para agregar paradas en tu Agregado
                newRoute.AddStop(stopId); 
            }
            try
            {
                await routeRepository.AddAsync(newRoute);
                await unitOfWork.CompleteAsync();
                await publishEndpoint.Publish<IRouteCreated>(new 
                {
                    RouteId = newRoute.Id,
                    CompanyId = newRoute.CompanyId,
                    RouteName = $"Ruta {newRoute.Id}", // O algún dato descriptivo
                    CreatedAt = DateTime.UtcNow
                });
                return newRoute;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creando ruta: {ex.Message}");
                return null;
            }
        }

        public async Task<RouteAggregate?> Handle(int idRoute, UpdateRouteCommand command)
        {
            var route = await routeRepository.FindByIdAsync(idRoute);
            if (route == null) return null;

            var updatedRoute = new RouteAggregate(command); // Solo datos de routes
            try
            {
                routeRepository.Update(updatedRoute);
                await unitOfWork.CompleteAsync();
                return updatedRoute;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task Handle(DeleteRouteCommand command)
        {
            var route = await routeRepository.FindByIdAsync(command.idRoute);
            if (route == null) return;
            try
            {
                routeRepository.Remove(route);
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}