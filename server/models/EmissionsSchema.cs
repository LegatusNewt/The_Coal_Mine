using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Npgsql.EntityFrameworkCore.PostgreSQL;

public class EmissionsDBContext : DbContext
{
    public DbSet<Emission> Emission { get; set; }
    public DbSet<Coverage> Coverage { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("<connection string>", o => o.useNetTopologySuite());
}


public class Emission
{
    public int Id { get; set; }
    public float Ch4 { get; set; }
    public float C2H6 { get; set; }
}
// At the point where you need to perform a database operation:
using var context = new EmissionsDBContext();