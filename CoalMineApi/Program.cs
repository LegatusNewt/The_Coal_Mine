using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

// Add the configuration
if (builder.Environment.IsDevelopment())
{
	builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
}
else
{
	builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
}

// Add the database context
builder.Services.AddDbContext<EmissionsDBContext>();
builder.Services.AddEntityFrameworkNpgsql();

// Add CORS services to allow all for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocal", policy =>
    {
		policy.AllowCredentials();
        policy.AllowAnyHeader();
		policy.AllowAnyMethod();
		policy.WithOrigins("http://localhost:3000");
    });
});


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
		options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
	});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowLocal");

// Seed the database with data from a CSV file if the environment is Development
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var environment = services.GetRequiredService<IHostEnvironment>();
	var AppDB = services.GetRequiredService<EmissionsDBContext>();
	var ConnectionString = AppDB.Database.GetDbConnection().ConnectionString;
	
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

