using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using CoalMineApi.Entities;
using Npgsql;

public class EmissionsDBContext : DbContext
{
    private readonly IConfiguration configuration;
    
    public EmissionsDBContext(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), o => o.UseNetTopologySuite());
        base.OnConfiguring(optionsBuilder);
    }
    public DbSet<Emission> Emissions { get; set; }
    public DbSet<Coverage> Coverages { get; set; }
}