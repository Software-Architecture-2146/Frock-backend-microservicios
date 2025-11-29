using MassTransit;
using Frock.Contracts;
using Frock_backend.suscriptions.domain.model.aggregates;
using suscriptions.shared.Infrastructure.Persistence.EFC.Configuration;

namespace Frock_backend.suscriptions.Consumers
{
    public class SuscriptionPlanConsumer : IConsumer<ITransportCompanyCreated>
    {
        private readonly AppDbContext _context;

        public SuscriptionPlanConsumer(AppDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<ITransportCompanyCreated> context)
        {
            var message = context.Message;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[RABBIT MQ] 🎁 Creando Plan Gratuito para: {message.Name}");
            Console.ResetColor();

            // Lógica: Crear suscripción "Free Trial"
            var newSubscription = new Suscription(
                companyId: message.Id, 
                planName: "Plan Gratuito - Trial", 
                price: 0.00
            );

            _context.Suscriptions.Add(newSubscription);
            await _context.SaveChangesAsync();

            Console.WriteLine($"--> Suscripción activada para la empresa {message.Name}");
        }
    }
}