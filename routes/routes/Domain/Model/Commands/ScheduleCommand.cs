namespace Frock_backend.routes.Domain.Model.Commands
{
    // Es solo una bolsita de datos, sin ID
    public record ScheduleCommand(
        string StartTime, 
        string EndTime, 
        string DayOfWeek, 
        bool Enabled
    );
}