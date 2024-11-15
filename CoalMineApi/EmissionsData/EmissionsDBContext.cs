using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using CoalMineApi.Entities;
using Npgsql;
using Microsoft.Extensions.Configuration;

public class EmissionsDBContext : DbContext
{
    private readonly IConfiguration _configuration;

    public EmissionsDBContext(DbContextOptions<EmissionsDBContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"), o => o.UseNetTopologySuite());
        }
        base.OnConfiguring(optionsBuilder);
    }

    public DbSet<Emission> Emissions { get; set; }
    public DbSet<Coverage> Coverages { get; set; }
}