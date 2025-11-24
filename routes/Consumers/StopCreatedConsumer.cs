using MassTransit;
using Frock.Contracts;
using Frock_backend.routes.Domain.Model.Entities;
using Frock_backend.shared.Infrastructure.Persistences.EFC.Configuration; 

namespace Frock_backend.routes.Consumers
{
    public class StopCreatedConsumer : IConsumer<IStopCreated>
    {
        private readonly AppDbContext _context;

        public StopCreatedConsumer(AppDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<IStopCreated> context)
        {
            var msg = context.Message;
            Console.WriteLine($"[RABBIT MQ] 🚏 Guardando Parada: {msg.Name}");

            var existing = await _context.Stops.FindAsync(msg.Id);
            if (existing != null) return;

            _context.Stops.Add(new Stop 
            { 
                Id = msg.Id, 
                Name = msg.Name, 
                Address = msg.Address 
            });
            await _context.SaveChangesAsync();
        }
    }
}