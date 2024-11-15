using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Create the database source builder
var ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dataSourceBuilder = new NpgsqlDataSourceBuilder(ConnectionString);

// Use NetTopologySuite for geometry types
dataSourceBuilder.UseNetTopologySuite();
var dataSource = dataSourceBuilder.Build();

// Add the database context
builder.Services.AddDbContext<EmissionsDBContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), 
	x => x.UseNetTopologySuite()));
builder.Services.AddEntityFrameworkNpgsql();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Seed the database with data from a CSV file if the environment is Development
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var environment = services.GetRequiredService<IHostEnvironment>();
	
	// Safety check
	// I want to only run the data seeding on localhost on startup
	// For testing we don't want to seed this data either, that should be done per test with factories
	if (environment.IsDevelopment() && ConnectionString.Contains("localhost"))
	{
		var context = services.GetRequiredService<EmissionsDBContext>();
		context.Database.Migrate(); // Apply any pending migrations
		var seeder = new DevDataSeeder(context);
		seeder.Seed();
	}
}

app.Run();