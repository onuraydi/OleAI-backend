using System.Text;
using System.Threading.RateLimiting;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using StackExchange.Redis;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddApplicationServices();
builder.Services.AddPersistanceServices(builder.Configuration);

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379"));

// CORE KATMANI SERVİSLERİ
builder.Services.AddScoped<ICacheService, RedisCacheManager>();
builder.Services.AddScoped<IMailService, SmtpMailManager>();
builder.Services.AddScoped<ITokenHelper, JwtHelper>();
builder.Services.AddSingleton<HttpExceptionHandler>();


builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Swagger Configuration

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "OleAI API", Version = "v1" });

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
    });

var app = builder.Build();

app.UseForwardedHeaders();

// kendi yazdığımız exceptionları entegre ettik.
app.UseMiddleware<ExceptionMiddleware>();

app.UseRateLimiter();

// ---- Middleware Pipeline ----
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

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

await app.RunAsync();
