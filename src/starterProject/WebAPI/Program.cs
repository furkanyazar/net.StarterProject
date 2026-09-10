using Application;
using Core.Application.Pipelines.Caching;
using Core.CrossCuttingConcerns.Exception.WebApi.Extensions;
using Core.CrossCuttingConcerns.Logging.Configurations;
using Core.Security.Encryption;
using Core.Security.JWT;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Persistence;
using WebAPI;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

const string fileLoggerConfigurationSection = "SeriLogConfigurations:FileLogConfiguration";
FileLogConfiguration fileLogConfiguration =
    builder.Configuration.GetSection(fileLoggerConfigurationSection).Get<FileLogConfiguration>()
    ?? throw new InvalidOperationException(
        $"\"{fileLoggerConfigurationSection}\" section cannot found in configuration."
    );
const string tokenOptionsConfigurationSection = "TokenOptions";
TokenOptions tokenOptions =
    builder.Configuration.GetSection(tokenOptionsConfigurationSection).Get<TokenOptions>()
    ?? throw new InvalidOperationException(
        $"\"{tokenOptionsConfigurationSection}\" section cannot found in configuration."
    );
const string redisOptionsSection = "RedisOptions";
RedisOptions redisOptions =
    builder.Configuration.GetSection(redisOptionsSection).Get<RedisOptions>()
    ?? throw new InvalidOperationException(
        $"\"{redisOptionsSection}\" section cannot found in configuration."
    );
const string webApiConfigurationSection = "WebAPIConfiguration";
WebAPIConfiguration webApiConfiguration =
    builder.Configuration.GetSection(webApiConfigurationSection).Get<WebAPIConfiguration>()
    ?? throw new InvalidOperationException(
        $"\"{webApiConfigurationSection}\" section cannot found in configuration."
    );

builder.Services.AddControllers();
builder.Services.AddApplicationServices(
    fileLogConfiguration: fileLogConfiguration,
    tokenOptions: tokenOptions
);
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddOpenApi();

builder.Services.AddStackExchangeRedisCache(opt => opt.Configuration = redisOptions.Configuration);

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = tokenOptions.Issuer,
            ValidAudience = tokenOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey),
        }
    );

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (app.Environment.IsProduction())
{
    app.ConfigureCustomExceptionMiddleware();
}

app.UseCors(opt =>
    opt.WithOrigins(webApiConfiguration.AllowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
);

app.UseAuthentication();

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Run();
