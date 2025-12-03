using MassTransit;
using Frock.Contracts;

namespace Frock_backend.suscriptions.Consumers
{
    public class RouteCreatedConsumer : IConsumer<IRouteCreated>
    {
        public Task Consume(ConsumeContext<IRouteCreated> context)
        {
            var msg = context.Message;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[RABBIT MQ] 📊 Suscriptions detectó nueva Ruta (ID: {msg.RouteId}) para la Empresa {msg.CompanyId}");
            Console.WriteLine("--> Aquí podríamos restar 1 crédito al plan de la empresa.");
            Console.ResetColor();
            return Task.CompletedTask;
        }
    }
}