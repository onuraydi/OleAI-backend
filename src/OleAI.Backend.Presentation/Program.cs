using System.Text;
using System.Threading.RateLimiting;
using DotNetEnv;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OleAI.Backend.Application;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching.Redis;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Handlers;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.MiddleWares;
using OleAI.Backend.Core.CrossCuttingConcerns.Mailing;
using OleAI.Backend.Core.Helpers;
using OleAI.Backend.Core.Security.Jwt;
using OleAI.Backend.Persistence;
using OleAI.Backend.Persistence.Contexts;
using OleAI.Backend.Presentation.Core.CrossCuttingConcerns.Monitoring;
using OleAI.Backend.Presentation.Middlewares;
using OleAI.Backend.Presentation.Swagger;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Serilog;
using Serilog.Exceptions;
using StackExchange.Redis;

if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Production")
{
    Env.TraversePath().Load();
}

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.WithExceptionDetails();
});

// CORS Config
builder.Services.AddCors(options =>
{
    options.AddPolicy("OleAI-Web",
        policy => policy
            .WithOrigins("http://localhost:5173", "https://oleai.com", "https://www.oleai.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// Cloudflare / Proxy IP Resolution
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Rate Limiting Config
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("AuthLimit", httpContext =>
    {
        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString();
        if (httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            clientIp = forwardedFor.FirstOrDefault();
        }

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: clientIp ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
    });
});

// ---- Service Registrations (katman bazlı extension methods) ----
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddPersistanceServices(builder.Configuration);

// Monitoring / Observability (OpenTelemetry)
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("OleAI.Backend"))
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddMeter("Microsoft.EntityFrameworkCore")
            .AddMeter("OleAI.HealthChecks")
            .AddPrometheusExporter();
    });

// Health Checks
builder.Services.AddHealthChecks()
    .AddSqlServer(
        connectionString: builder.Configuration.GetConnectionString("OleAIConnectionString") ?? throw new InvalidOperationException("Connection string 'OleAIConnectionString' not found."),
        name: "SQL Server",
        tags: new[] { "db", "sql", "sqlserver" }
    );

builder.Services.AddHostedService<HealthCheckMetricsService>();

// Hangfire Config
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("OleAIConnectionString")));

builder.Services.AddHangfireServer();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379"));

// CORE KATMANI SERVİSLERİ
builder.Services.AddScoped<ICacheService, RedisCacheManager>();
builder.Services.AddScoped<IMailService, SmtpMailManager>();
builder.Services.AddScoped<ITokenHelper, JwtHelper>();
builder.Services.AddSingleton<HttpExceptionHandler>();


builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddOpenApi();

// Swagger Configuration

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "OleAI API", Version = "v1" });

    opt.OperationFilter<CreateRecipeFormOperationFilter>();

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Token bilginizi 'Bearer {token}' formatına gerek kalmadan doğrudan aşağıya giriniz."
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });

    // XML açıklamalarını Swagger'a ekle
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, xmlFile);
    opt.IncludeXmlComments(xmlPath);
});

// JWT Configuration

var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = tokenOptions!.Issuer,
            ValidAudience = tokenOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey)),

            // SENIOR CHECK: Default 5 dakikalık tolerans süresini sıfırlıyoruz. Bu sayede tam 15 dk'de token süresi doluyor.
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["accessToken"];
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

app.UseForwardedHeaders();

app.UseCorrelationId();

// kendi yazdığımız exceptionları entegre ettik.
app.UseMiddleware<ExceptionMiddleware>();

app.UseSerilogRequestLogging(options =>
{
    // Log the CorrelationId along with request details if needed, though it's already in the context
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});

app.UseRateLimiter();

// ---- Middleware Pipeline ----
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("OleAI-Web");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Prometheus Scraping Endpoint
app.MapPrometheusScrapingEndpoint("/metrics");

// Health Check Endpoints
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Hangfire Dashboard (Development only or protected in Prod)
if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/hangfire");
}

// Automatic Migration at Startup (Development only)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        await dbContext.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

try
{
    await DbSeeder.SeedAsync(app.Services);
    Console.WriteLine("Veritabanı kontrol edildi/tohumlandı.");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Veritabanı tohumlanırken hata oluştu.");
}

try
{
    var firebaseCredentialsJson = builder.Configuration["Firebase:CredentialsJson"];
    var firebaseCredentialsPath = builder.Configuration["Firebase:CredentialsPath"];

    AppOptions? options = null;

    if (!string.IsNullOrEmpty(firebaseCredentialsJson))
    {
        options = new AppOptions
        {
            Credential = GoogleCredential.FromJson(firebaseCredentialsJson)
        };
    }
    else if (!string.IsNullOrEmpty(firebaseCredentialsPath))
    {
        var absolutePath = System.IO.Path.IsPathRooted(firebaseCredentialsPath)
            ? firebaseCredentialsPath
            : System.IO.Path.Combine(System.AppContext.BaseDirectory, firebaseCredentialsPath);

        if (System.IO.File.Exists(absolutePath))
        {
            options = new AppOptions
            {
                Credential = GoogleCredential.FromFile(absolutePath)
            };
        }
        else
        {
            Log.Warning("Firebase credentials file not found at {path}", absolutePath);
        }
    }

    if (options != null)
    {
        FirebaseApp.Create(options);
    }
    else
    {
        // Fallback to GOOGLE_APPLICATION_CREDENTIALS
        FirebaseApp.Create();
    }

    Log.Information("FirebaseApp initialized successfully.");
}
catch (Exception ex)
{
    Log.Warning(ex, "FirebaseApp initialization failed. Push notifications may not work. Make sure GOOGLE_APPLICATION_CREDENTIALS or Firebase:CredentialsPath is set.");
}

// Background Jobs Scheduling
RecurringJob.AddOrUpdate<OleAI.Backend.Application.Features.Notifications.Jobs.ExpirationNotifierJob>(
    "expiration-notifier",
    job => job.ExecuteAsync(),
    Cron.Daily);

await app.RunAsync();
