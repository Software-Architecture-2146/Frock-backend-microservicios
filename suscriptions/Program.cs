using Microsoft.EntityFrameworkCore;
using Frock.Contracts; // Para la extensión de RabbitMQ
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.OpenApi.Models;

// USINGS DE TU PROYECTO (Ajustados a lo que creamos antes)
using suscriptions.shared.Infrastructure.Persistence.EFC.Configuration; // Tu AppDbContext
using Frock_backend.suscriptions.Consumers;
using Microsoft.OpenApi; // Tu Consumidor
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
var builder = WebApplication.CreateBuilder(args);
// Limpiar claims
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// Leer secreto
var secretKey = builder.Configuration["TokenSettings:Secret"] ?? "EstaEsUnaClaveSuperSecretaYMuyLargaParaFrockBackend2025";
var key = Encoding.ASCII.GetBytes(secretKey);

// Configurar JWT
builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
// --- 1. Configuración de Servicios ---

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Necesario para Swagger clásico

// Configuración de Swagger (Título y Versión)
// Configuración de Swagger con botón Authorize
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Suscriptions API",
        Version = "v1",
        Description = "Microservicio de Suscripciones y Planes"
    });

    // --- ESTO ES LO QUE FALTABA ---
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    // -----------------------------
});

// --- 2. Base de Datos (MySQL) ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (connectionString != null) 
        options.UseMySQL(connectionString);
});

// --- 3. RabbitMQ (MassTransit) ---
// Registramos el consumidor que creamos para escuchar a Transport Company
builder.Services.AddRabbitMqBus(typeof(SuscriptionPlanConsumer), typeof(RouteCreatedConsumer));

var app = builder.Build();

// --- 4. Crear Base de Datos Automáticamente ---
// Esto creará la tabla 'suscriptions' al arrancar si no existe
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try 
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error creando la DB: {ex.Message}");
    }
}

// --- 5. Configuración del Pipeline HTTP ---

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    c.DocExpansion(DocExpansion.None);
    c.DocumentTitle = "Suscriptions API"; // Nombre de la pestaña del navegador
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();