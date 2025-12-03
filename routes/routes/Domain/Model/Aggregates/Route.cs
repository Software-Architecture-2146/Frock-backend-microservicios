using Frock_backend.routes.Domain.Model.Commands;
using Frock_backend.routes.Domain.Model.Entities;

namespace Frock_backend.routes.Domain.Model.Aggregates
{
    public class RouteAggregate
    {
        public int Id { get; set; }
        public int CompanyId { get; set; } 
        public double Price { get; set; }
        public int Duration { get; set; }
        public int Frequency { get; set; }
        

        public ICollection<RoutesStops> Stops { get; set; } = new List<RoutesStops>();
        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
        
        public RouteAggregate() { }
        public RouteAggregate(int companyId, double price, int duration, int frequency)
        {
            CompanyId = companyId;
            Price = price;
            Duration = duration;
            Frequency = frequency;
            Stops = new List<RoutesStops>(); // Inicializamos la lista
        }
        

        // Este es el constructor que usa tu Controller cuando creas una ruta
        public RouteAggregate(CreateFullRouteCommand cm)
        {
            CompanyId = cm.CompanyId;
            Price = cm.Price;
            Duration = cm.Duration;
            Frequency = cm.Frequency;
            Stops = new List<RoutesStops>();
            // -------------------------------

            foreach (var stopId in cm.StopsIds)
            {
                AddStop(stopId); 
            }

            foreach (var schedule in cm.Schedules)
            {
                AddSchedule(schedule.StartTime, schedule.EndTime, schedule.DayOfWeek, schedule.Enabled);
            }
        }

        public RouteAggregate(UpdateRouteCommand cm)
        {
            Price = cm.Price;
            Duration = cm.Duration;
            Frequency = cm.Frequency;

            Stops = new List<RoutesStops>(); 
            foreach (var stopId in cm.StopsIds)
            {
                AddStop(stopId);
            }

            // Igual para Schedules
            Schedules = new List<Schedule>();
            foreach (var schedule in cm.Schedules)
            {
                AddSchedule(schedule.StartTime, schedule.EndTime, schedule.DayOfWeek, schedule.Enabled);
            }
        }

        public RouteAggregate(DeleteRouteCommand cm)
        {
            Id = cm.idRoute;
        }

        public void AddStop(int stopId)
        {
            // Crea la relación y la agrega a la lista
            var routeStop = new RoutesStops
            {
                FkStopId = stopId
                // FKRouteId se llenará automáticamente al guardar porque es hijo de esta entidad
            };
            
            this.Stops.Add(routeStop);
        }
        public void AddSchedule(string start, string end, string dayOfWeek, bool enabled)
        {
            Schedules.Add(new Schedule(start, end, dayOfWeek, enabled));
        }
    }
}