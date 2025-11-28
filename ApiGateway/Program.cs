using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1. CRUCIAL: Cargar el archivo de configuración "ocelot.json"
// Sin esto, el Gateway no sabe a dónde redirigir el tráfico.
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// 2. Inyectar el servicio de Ocelot usando esa configuración
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

// 3. Activar el middleware de Ocelot
await app.UseOcelot();

app.Run();