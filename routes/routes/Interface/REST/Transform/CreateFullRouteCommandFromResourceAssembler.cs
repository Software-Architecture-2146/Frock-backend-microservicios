using Frock_backend.routes.Domain.Model.Commands;
using Frock_backend.routes.Interface.REST.Resources;
using Frock_backend.routes.Domain.Model.Entities;
namespace Frock_backend.routes.Interface.REST.Transform
{
    public class CreateFullRouteCommandFromResourceAssembler
    {
        public static CreateFullRouteCommand toCommandFromResource(CreateFullRouteResource resource, int userId) =>
            new CreateFullRouteCommand(
                resource.CompanyId,
                resource.Price,
                resource.Duration,
                resource.Frequency,
                resource.StopsIds,
                resource.Schedules.Select(schedule => new ScheduleCommand(
                    schedule.StartTime,
                    schedule.EndTime,
                    schedule.DayOfWeek,
                    schedule.Enabled
                )).ToList(),
                userId
            );
    }
}
