using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Frock_backend.shared.Infrastructure.Persistences.EFC.Configuration;
using Frock_backend.shared.Infrastructure.Persistences.EFC.Repositories;
using Frock_backend.shared.Infrastructure.Interfaces.ASP.Configuration;
using Frock_backend.shared.Domain.Repositories;

using Frock_backend.IAM.Application.Internal.CommandServices;
using Frock_backend.IAM.Application.Internal.OutboundServices;
using Frock_backend.IAM.Application.Internal.QueryServices;

using Frock_backend.IAM.Domain.Repositories;
using Frock_backend.IAM.Domain.Services;
using Frock_backend.IAM.Infrastructure.Persistence.EFC.Repositories;

using Frock_backend.IAM.Infrastructure.Hashing.BCrypt.Services;
using Frock_backend.IAM.Infrastructure.Pipeline.Middleware.Extensions;
using Frock_backend.IAM.Infrastructure.Tokens.JWT.Configuration;
using Frock_backend.IAM.Infrastructure.Tokens.JWT.Services;

using Frock_backend.IAM.Interfaces.ACL;
using Frock_backend.IAM.Interfaces.ACL.Services;

using Frock_backend.shared.Domain.Services;
using Frock_backend.shared.Infrastructure.Configuration;
using Frock_backend.shared.Infrastructure.Services;
using Frock.Contracts;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURACIÓN DE RUTAS Y CONTROLADORES ---
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers(options => 
    {
        // Mantenemos tu configuración de rutas bonitas
        options.Conventions.Add(new KebabCaseRouteNamingConvention());
    })
    .AddJsonOptions(options => // <--- Aquí agregamos la magia del JSON
    {
        // Esto permite enviar "Admin" en vez de 2 en el JSON
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// --- 2. CONFIGURACIÓN DE SWAGGER ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "IAM API", // <--- CAMBIO: Nombre específico del microservicio
        Version = "v1",
        Description = "Frock Identity & Access Management API",
        TermsOfService = new Uri("https://acme-learning.com/tos"),
        Contact = new OpenApiContact { Name = "Frock Studios", Email = "contact@frock.com" },
        License = new OpenApiLicense { Name = "Apache 2.0", Url = new Uri("https://www.apache.org/licenses/LICENSE-2.0.html") }
    });
    
    // Configuración del Candado (Bearer Token)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme }
            },
            Array.Empty<string>()
        }
    });
});

// --- 3. BASE DE DATOS ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (connectionString is null) throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    // Ajusta esto según si es Dev o Prod, aquí lo simplifiqué para que funcione siempre
    if (builder.Environment.IsDevelopment())
    {
        options.UseMySQL(connectionString)
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    }
    else
    {
        options.UseMySQL(connectionString)
            .LogTo(Console.WriteLine, LogLevel.Error)
            .EnableDetailedErrors();
    }
});

// --- 4. INYECCIÓN DE DEPENDENCIAS ---
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// IAM Services
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IHashingService, HashingService>();
builder.Services.AddScoped<IIamContextFacade, IamContextFacade>();

// Cloudinary
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

// Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// --- 5. SEGURIDAD JWT (ESTO FALTABA) ---
// Esto permite validar el token que llega
var tokenSettings = builder.Configuration.GetSection("TokenSettings").Get<TokenSettings>();
// Si tokenSettings es nulo, usaremos un valor default para que no explote al compilar, 
// pero asegúrate de tenerlo en appsettings.json
var secretKey = tokenSettings?.Secret ?? "SecretKeyTemporalParaDesarrollo123456"; 
var key = Encoding.ASCII.GetBytes(secretKey);

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

// --------------------------------------------
// Habilitar RabbitMQ para poder enviar mensajes
builder.Services.AddRabbitMqBus();
var app = builder.Build();

app.UseCors();

// Crear DB automáticamente
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

// Pipeline
app.UseSwagger(c =>
{
    c.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0;
});
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    // c.RoutePrefix = string.Empty; // Comentado para consistencia con los otros microservicios
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    c.DocumentTitle = "IAM API"; // Nombre de la pestaña
});

app.UseHttpsRedirection();
app.UseRouting();

// Middleware de Auth
app.UseRequestAuthorization(); // Tu middleware personalizado
app.UseAuthentication(); // <--- Verifica quién eres (JWT)
app.UseAuthorization();  // <--- Verifica qué puedes hacer

app.MapControllers();

app.Run();