using MassTransit;
using Frock.Contracts;
using Frock_backend.shared.Infrastructure.Persistences.EFC.Configuration;
using Frock_backend.transport_Company.Domain.Model.Entities; // Asegúrate de que este sea el namespace donde creaste tu entidad User

namespace Frock_backend.transport_Company.Consumers
{
    public class UserCreatedConsumer : IConsumer<IUserCreated>
    {
        private readonly AppDbContext _context;

        // Inyectamos el DbContext para poder guardar en la base de datos
        public UserCreatedConsumer(AppDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<IUserCreated> context)
        {
            var msg = context.Message;
            
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"[RABBIT MQ] 👤 Nuevo Usuario detectado: {msg.Username} (ID: {msg.Id})");
            Console.ResetColor();

            // 1. Verificamos si ya existe para no duplicar (Idempotencia)
            var existingUser = await _context.Users.FindAsync(msg.Id);
            if (existingUser != null)
            {
                return;
            }

            // 2. Creamos la copia local del usuario
            var newUser = new User
            {
                Id = msg.Id,          // El ID viene de IAM
                Username = msg.Username
            };

            // 3. Guardamos en la base de datos de Transport
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            
            Console.WriteLine("--> Usuario replicado en Transport Company exitosamente.");
        }
    }
}