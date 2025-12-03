namespace Frock_backend.routes.Domain.Model.Entities
{
    public class RoutesStops
    {
        public int Id { get; set; }
        public int FkStopId { get; set; }
        public int FKRouteId { get; set; }
        public RoutesStops(){}

        public RoutesStops(int fkStopId, int fkRouteId)
        {
            FkStopId = fkStopId;
            FKRouteId = fkRouteId;
        }
    }
}