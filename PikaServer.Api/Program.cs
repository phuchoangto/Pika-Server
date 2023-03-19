using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using PikaServer.Api.ServiceExtensions;
using PikaServer.Persistence.Context;
using PikaServer.ServiceExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.ConfigStartup(builder.Configuration);

var app = builder.Build();
app.Services.PostExecution();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.DisplayOperationId();
		c.DisplayRequestDuration();
	});
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// using (var scope = app.Services.CreateScope())
// {
// 	var services = scope.ServiceProvider;
//
// 	var context = services.GetRequiredService<AppDbContext>();
// 	if (context.Database.GetPendingMigrations().Any())
// 	{
// 		context.Database.Migrate();
// 	}
// }

app.Run();
