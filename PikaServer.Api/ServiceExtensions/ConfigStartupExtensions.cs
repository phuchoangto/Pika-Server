using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using PikaServer.Infras;
using PikaServer.Persistence;

namespace PikaServer.Api.ServiceExtensions;

public static class ConfigStartupExtensions
{
	/// <summary>
	///     Config service for application startup
	/// </summary>
	/// <param name="services"></param>
	/// <param name="configuration"></param>
	/// <returns></returns>
	public static IServiceCollection ConfigStartup(this IServiceCollection services, IConfiguration configuration)
    {
        RegisterAppSettings(services, configuration);
        RegisterServices(services, configuration);
        ConfigSwaggerGen(services);

        return services;
    }

    private static IServiceCollection RegisterAppSettings(IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }

    private static IServiceCollection RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.UseHdBankVendor(opt => configuration.GetSection("HDBankApi").Bind(opt));
        services.UseJwtAuthentication(config => configuration.GetSection("JwtAuth").Bind(config));
        services.UseDbPersistence(configuration.GetValue<string>("MsSql:ConnectionString"));
        services.UseNotification(config => configuration.GetSection("Notification").Bind(config));

        return services;
    }

    private static IServiceCollection ConfigSwaggerGen(IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            var jwtSchema = new OpenApiSecurityScheme
            {
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "Jwt",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Description = "Paste ONLY `jwt` here",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };
            c.AddSecurityDefinition(jwtSchema.Reference.Id, jwtSchema);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { jwtSchema, ArraySegment<string>.Empty }
            });
        });

        return services;
    }
}