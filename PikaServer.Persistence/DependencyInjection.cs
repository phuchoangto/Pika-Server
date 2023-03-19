using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PikaServer.Persistence.Context;
using PikaServer.Persistence.Internal.Abstracts;
using PikaServer.Persistence.Internal.Data;

namespace PikaServer.Persistence;

public static class DependencyInjection
{
	public static IServiceCollection UseDbPersistence(this IServiceCollection services, string connStr)
	{
		services.AddDbContext<AppDbContext>(opts =>
			opts.UseSqlServer(connStr,
				sqlOpt => sqlOpt.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName))
		);

		services.AddTransient<IUnitOfWork, UnitOfWork>();

		return services;
	}
}
