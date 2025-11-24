using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Frock.Contracts
{
    public static class MassTransitExtensions
    {
        // CAMBIO CLAVE: 'params Type[] consumers' permite enviar 0, 1, o muchos consumidores
        public static void AddRabbitMqBus(this IServiceCollection services, params Type[] consumers)
        {
            services.AddMassTransit(x =>
            {
                // Recorremos la lista y registramos todos los consumidores que hayas enviado
                foreach (var consumer in consumers)
                {
                    x.AddConsumer(consumer);
                }

                x.UsingRabbitMq((context, cfg) =>
                {
                    // Configuración para conectar con tu Docker
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                    
                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }
}