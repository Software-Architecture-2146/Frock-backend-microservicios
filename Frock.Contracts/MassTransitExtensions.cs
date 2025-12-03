using MassTransit;
using Microsoft.Extensions.Configuration; // <--- NECESARIO
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Frock.Contracts
{
    public static class MassTransitExtensions
    {
        public static void AddRabbitMqBus(this IServiceCollection services, params Type[] consumers)
        {
            services.AddMassTransit(x =>
            {
                foreach (var consumer in consumers)
                {
                    x.AddConsumer(consumer);
                }

                x.UsingRabbitMq((context, cfg) =>
                {
                    // --- EL CAMBIO CLAVE ESTÁ AQUÍ ---
                    
                    // 1. Obtenemos la configuración del sistema
                    var configuration = context.GetRequiredService<IConfiguration>();
                    
                    // 2. Buscamos si Docker nos dijo dónde está el conejo (RabbitMq:Host)
                    // Si no nos dijo nada (es null), asumimos que estamos en local ("localhost")
                    var rabbitMqHost = configuration["RabbitMq:Host"] ?? "localhost";

                    // 3. Usamos esa dirección dinámica
                    cfg.Host(rabbitMqHost, "/", h =>
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