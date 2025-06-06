using AspNetCoreRateLimit;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WeatherEmergencyAPI.Consumers;
using WeatherEmergencyAPI.Data;
using WeatherEmergencyAPI.Configurations;
using WeatherEmergencyAPI.Middleware;
using WeatherEmergencyAPI.ML;
using WeatherEmergencyAPI.Repositories;
using WeatherEmergencyAPI.Repositories.Interfaces;
using WeatherEmergencyAPI.Services;
using WeatherEmergencyAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Adicionar CORS - IMPORTANTE: deve ser um dos primeiros services
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactNativePolicy",
        policy => policy
            .AllowAnyOrigin()    // Permite qualquer origem
            .AllowAnyMethod()    // Permite GET, POST, PUT, DELETE, etc.
            .AllowAnyHeader()    // Permite qualquer header
            .WithExposedHeaders("X-Rate-Limit-Limit", "X-Rate-Limit-Remaining", "X-Rate-Limit-Reset")); // Expõe headers de rate limit
});

// Para desenvolvimento, aceitar conexões de qualquer IP
if (builder.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("http://0.0.0.0:5165", "https://0.0.0.0:7013");
}

// Configurar Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<ClientRateLimitOptions>(builder.Configuration.GetSection("ClientRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Configurar Oracle Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseOracle(
        builder.Configuration.GetConnectionString("OracleConnection"),
        oracleOptions => oracleOptions.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19)
    ));

// Configurar AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Registrar Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IEmergencyContactRepository, EmergencyContactRepository>();

// Registrar Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IEmergencyContactService, EmergencyContactService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMessageBusService, MessageBusService>();

// Configurar HttpClient para Weather Service
builder.Services.AddHttpClient<IWeatherService, WeatherService>();

// Registrar ML Service
builder.Services.AddSingleton<MLModelService>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<MLModelService>>();
    var environment = provider.GetRequiredService<IWebHostEnvironment>();
    return new MLModelService(logger, environment);
});

// Configurar MassTransit e RabbitMQ
builder.Services.AddMassTransit(x =>
{
    // Registrar Consumers
    x.AddConsumer<UserRegisteredConsumer>();
    x.AddConsumer<WeatherAlertConsumer>();
    x.AddConsumer<LogEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMQConfig = builder.Configuration.GetSection("RabbitMQ");

        cfg.Host(rabbitMQConfig["Host"], Convert.ToUInt16(rabbitMQConfig["Port"]), rabbitMQConfig["VirtualHost"], h =>
        {
            h.Username(rabbitMQConfig["Username"]);
            h.Password(rabbitMQConfig["Password"]);
        });

        // Configurar endpoints para cada consumer
        cfg.ReceiveEndpoint("user-registered-queue", e =>
        {
            e.ConfigureConsumer<UserRegisteredConsumer>(context);
        });

        cfg.ReceiveEndpoint("weather-alert-queue", e =>
        {
            e.ConfigureConsumer<WeatherAlertConsumer>(context);
        });

        cfg.ReceiveEndpoint("log-event-queue", e =>
        {
            e.ConfigureConsumer<LogEventConsumer>(context);
        });

        // Configurar retry policy
        cfg.UseMessageRetry(r => r.Intervals(100, 200, 500, 800, 1000));
    });
});

// Configurar JWT Authentication
var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddControllers();

// Configurar Swagger com autenticação JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Weather Emergency API",
        Version = "v1",
        Description = "API para gerenciamento de alertas climáticos e contatos de emergência com RabbitMQ e ML.NET",
        Contact = new OpenApiContact
        {
            Name = "Seu Nome",
            Email = "seu-email@example.com"
        }
    });

    // Configurar autenticação JWT no Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header usando o esquema Bearer. \r\n\r\n" +
                      "Digite 'Bearer' [espaço] e então seu token no campo abaixo.\r\n\r\n" +
                      "Exemplo: \"Bearer 12345abcdef\""
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

    // Incluir comentários XML na documentação
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather Emergency API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "Weather Emergency API - Documentação";
    });

    // Adicionar redirecionamento de /swagger para /swagger/index.html
    app.Use(async (context, next) =>
    {
        if (context.Request.Path == "/swagger")
        {
            context.Response.Redirect("/swagger/index.html", permanent: false);
            return;
        }
        await next();
    });
}

// IMPORTANTE: Em desenvolvimento, comentar UseHttpsRedirection para facilitar React Native
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Adicionar CORS - IMPORTANTE: deve vir ANTES de Authentication
app.UseCors("ReactNativePolicy");

// Adicionar Rate Limiting ANTES da autenticação
app.UseIpRateLimiting();
app.UseClientRateLimiting();
app.UseCustomRateLimit();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Adicionar endpoint de teste para verificar se API está acessível
if (app.Environment.IsDevelopment())
{
    app.MapGet("/", () => Results.Ok(new
    {
        message = "Weather Emergency API está rodando!",
        environment = app.Environment.EnvironmentName,
        timestamp = DateTime.UtcNow,
        endpoints = new
        {
            swagger = "/swagger",
            health = "/api/health",
            register = "/api/auth/register",
            login = "/api/auth/login"
        }
    })).ExcludeFromDescription();
}

app.Run();