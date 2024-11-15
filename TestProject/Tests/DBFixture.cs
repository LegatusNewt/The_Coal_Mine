using CoalMineApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;

public class EmissionsDbContextFixture : IDisposable
{
    public EmissionsDBContext Context { get; private set; }

    public EmissionsDbContextFixture()
    {
        var options = new DbContextOptionsBuilder<EmissionsDBContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique database name for each test run
            .Options;

        var configuration = new ConfigurationBuilder()
            .Build();

        Context = new EmissionsDBContext(options, configuration);

        // Ensure the database is empty before seeding
        ClearDatabase();

        // Seed the database with test data
        SeedDatabase();
    }

    private void ClearDatabase()
    {
        Context.Coverages.RemoveRange(Context.Coverages);
        Context.Emissions.RemoveRange(Context.Emissions);
        Context.SaveChanges();
    }

    private void SeedDatabase()
    {
        //Seeding of Test emissions data and buffers associated with those emissions

        var WKTReader = new WKTReader();
        var emissions = new List<Emission>
        {
            new Emission { Id = 1, CH4 = (float)1.1, C2H6 = (float)2.2, TimeStamp = DateTime.UtcNow, Point = new NetTopologySuite.Geometries.Point(1, 1) },
            new Emission { Id = 2, CH4 = (float)3.3, C2H6 = (float)4.4, TimeStamp = DateTime.UtcNow, Point = new NetTopologySuite.Geometries.Point(2, 2) }                
        };

        int bufferSize = 5;
        var coverages = emissions.Select(e => new Coverage
        {
            Id = e.Id,
            BufferSize = bufferSize,
            Description = $"Buffer around Emission {e.Id}",
            Geometry = (Polygon)e.Point.Buffer(bufferSize)
        }).ToList();
        
        Context.Emissions.AddRange(emissions);
        Context.Coverages.AddRange(coverages);
        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}