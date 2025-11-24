using Frock.Contracts;
using MassTransit;


using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Frock_backend.shared.Infrastructure.Persistences.EFC.Configuration;
using Frock_backend.shared.Infrastructure.Persistences.EFC.Repositories;
using Frock_backend.shared.Infrastructure.Interfaces.ASP.Configuration;
using Frock_backend.shared.Domain.Repositories;

using Frock_backend.stops.Application.Internal.CommandServices;
using Frock_backend.stops.Application.Internal.QueryServices;

using Frock_backend.stops.Domain.Repositories;
using Frock_backend.stops.Domain.Services;

using Frock_backend.stops.Infrastructure.Repositories;
using Frock_backend.stops.Application.Internal.CommandServices.Geographic;
using Frock_backend.stops.Application.Internal.QueryServices.Geographic;

using Frock_backend.stops.Domain.Repositories.Geographic;
using Frock_backend.stops.Domain.Services.Geographic;

using Frock_backend.stops.Infrastructure.Repositories.Geographic;

using Frock_backend.stops.Infrastructure.Seeding;
using Frock_backend.shared.Domain.Services;
using Frock_backend.shared.Infrastructure.Configuration;
using Frock_backend.shared.Infrastructure.Services;
using Frock_backend.stops.Application.External;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Configure Kebab Case Route Naming Convention
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
     options.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Frock_Backend",
            Version = "v1",
            Description = "Frock Backend API",
            TermsOfService = new Uri("https://acme-learning.com/tos"),
            Contact = new OpenApiContact
            {
                Name = "frock Studios",
                Email = "frockWEB.com"
            },
            License = new OpenApiLicense
            {
                Name = "Apache 2.0",
                Url = new Uri("https://www.apache.org/licenses/LICENSE-2.0.html")
            }
        });
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
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (connectionString is null)
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

// Configure Database Context and Logging Levels
if (builder.Environment.IsDevelopment())
    builder.Services.AddDbContext<AppDbContext>(
        options =>
        {
            options.UseMySQL(connectionString)
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        });
else if (builder.Environment.IsProduction())
    builder.Services.AddDbContext<AppDbContext>(
        options =>
        {
            options.UseMySQL(connectionString)
                .LogTo(Console.WriteLine, LogLevel.Error)
                .EnableDetailedErrors();
        });

// Shared Bounded Context Injection Configuration
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Geographic
builder.Services.AddScoped<IRegionRepository, RegionRepository>();
    builder.Services.AddScoped<IRegionCommandService, RegionCommandService>();
    builder.Services.AddScoped<IRegionQueryService, RegionQueryService>();
        /**/
    builder.Services.AddScoped<IProvinceRepository, ProvinceRepository>();
    builder.Services.AddScoped<IProvinceCommandService, ProvinceCommandService>();
    builder.Services.AddScoped<IProvinceQueryService, ProvinceQueryService>();
        /**/
    builder.Services.AddScoped<IDistrictRepository, DistrictRepository>();
    builder.Services.AddScoped<IDistrictCommandService, DistrictCommandService>();
    builder.Services.AddScoped<IDistrictQueryService, DistrictQueryService>();
        /**/

//Stops
    builder.Services.AddScoped<IStopRepository, StopRepository>();
    builder.Services.AddScoped<IStopCommandService, StopCommandService>();
    builder.Services.AddScoped<IStopQueryService, StopQueryService>();


//GEOSERVICE
    builder.Services.AddHttpClient<IGeoImportService, GeoImportService>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["GeoApi:BaseUrl"]);
    });
//Seeding Service Geographic Data
// Datos iniciales fijos de datos geográficos
builder.Services.AddScoped<GeographicDataSeeder>();


//CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://deft-tapioca-c27a9c.netlify.app", "https://frock-front-end.vercel.app")//ajustar
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Cloudinary Configuration
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

builder.Services.AddRabbitMqBus();

var app = builder.Build();

app.UseCors();

// Verify Database Objects are created
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();

  // Seed initial geographic data
    try
    {
        var seeder = services.GetRequiredService<GeographicDataSeeder>();
        await seeder.SeedDataAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error durante la carga de datos iniciales.");
    }
}

// Configure the HTTP request pipeline.
app.UseSwagger(c =>
{
    c.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0;
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    //c.RoutePrefix = string.Empty; // Opcional: para que Swagger sea la p�gina ra�z
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    c.DocumentTitle = "Stops API";
});

app.UseHttpsRedirection();
app.UseRouting(); // Si no está implícito
app.UseAuthorization(); // Authorization de ASP.NET Core
app.MapControllers();

app.Run();
