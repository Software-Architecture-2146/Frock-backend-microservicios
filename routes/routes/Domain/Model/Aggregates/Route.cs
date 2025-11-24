using Frock_backend.routes.Domain.Model.Commands;
using Frock_backend.routes.Domain.Model.Entities;

namespace Frock_backend.routes.Domain.Model.Aggregates
{
    public class RouteAggregate
    {
        public int Id { get; set; }
        public double Price { get; set; }
        public int Duration { get; set; }
        public int Frequency { get; set; }
        
        // --- 1. NUEVA PROPIEDAD AGREGADA ---
        public Guid CompanyId { get; set; } 
        // -----------------------------------

        public List<Schedule> Schedules = new();
        public List<RoutesStops> Stops = new();

        public RouteAggregate(double price, int duration, int frequency)
        {
            Price = price;
            Duration = duration;
            Frequency = frequency;
        }

        public void AddSchedule(string start, string end, string dayOfWeek, bool enabled)
        {
            Schedules.Add(new Schedule(start, end, dayOfWeek, enabled));
        }

        // Este es el constructor que usa tu Controller cuando creas una ruta
        public RouteAggregate(CreateFullRouteCommand cm)
        {
            Price = cm.Price;
            Duration = cm.Duration;
            Frequency = cm.Frequency;
            
            // --- 2. ASIGNAMOS LA EMPRESA ---
            // IMPORTANTE: Esto te dará error rojo hasta que modifiquemos el archivo del Command.
            // No te asustes, lo arreglamos en el siguiente paso.
            CompanyId = cm.CompanyId; 
            // -------------------------------

            foreach (var stopId in cm.StopsIds)
                Stops.Add(new RoutesStops(stopId));

            foreach (var schedule in cm.Schedules)
                AddSchedule(schedule.StartTime, schedule.EndTime, schedule.DayOfWeek, schedule.Enabled);
        }

        public RouteAggregate(UpdateRouteCommand cm)
        {
            Price = cm.Price;
            Duration = cm.Duration;
            Frequency = cm.Frequency;

            foreach (var stopId in cm.StopsIds)
                Stops.Add(new RoutesStops(stopId));

            foreach (var schedule in cm.Schedules)
                AddSchedule(schedule.StartTime, schedule.EndTime, schedule.DayOfWeek, schedule.Enabled);
        }

        public RouteAggregate(DeleteRouteCommand cm)
        {
            Id = cm.idRoute;
        }
    }
}