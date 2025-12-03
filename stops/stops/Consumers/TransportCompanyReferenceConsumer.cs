using MassTransit;
using Frock.Contracts;
using Frock_backend.shared.Infrastructure.Persistences.EFC.Configuration;
using Frock_backend.stops.Domain.Model.Entities;

namespace Frock_backend.stops.Consumers
{
    public class TransportCompanyReferenceConsumer : IConsumer<ITransportCompanyCreated>
    {
        private readonly AppDbContext _context;

        public TransportCompanyReferenceConsumer(AppDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<ITransportCompanyCreated> context)
        {
            var msg = context.Message;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"[RABBIT MQ] 🚛 Stops recibió empresa: {msg.Name}");
            Console.ResetColor();

            var existing = await _context.Companies.FindAsync(msg.Id);
            if (existing != null) return;

            _context.Companies.Add(new Company 
            { 
                Id = msg.Id, 
                Name = msg.Name 
            });
            
            await _context.SaveChangesAsync();
        }
    }
}