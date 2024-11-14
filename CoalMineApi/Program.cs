using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var dataSourceBuilder = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("DefaultConnection"));
dataSourceBuilder.UseNetTopologySuite();
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<EmissionsDBContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), 
	x => x.UseNetTopologySuite()));
builder.Services.AddEntityFrameworkNpgsql();
var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var context = services.GetRequiredService<EmissionsDBContext>();
	context.Database.Migrate();
	var seeder = new DevDataSeeder(context);
	seeder.Seed();
}

app.Run();