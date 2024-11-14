using CsvHelper;
using NetTopologySuite.Geometries;
using System.Globalization;

public class DevDataSeeder
{
    private readonly EmissionsDBContext _context;

    public DevDataSeeder(EmissionsDBContext context)
    {
        _context = context;
    }

    public void Seed()
    {
        SeedEmissions();
    }

    public void SeedEmissions()
    {
        if (!_context.Emissions.Any())
        {
            var emissions = ReadCsvToEmissions();
            _context.Emissions.AddRange(emissions);
            _context.SaveChanges();
        }
    }

    private List<Emission> ReadCsvToEmissions()
    {
        // For testing purposes before DB is set up
        string filePath = "./EmissionsData/data.csv";
        var records = new List<Emission>();
        using var reader = new StreamReader(filePath);
        using(var csv = new CsvReader(reader, CultureInfo.CurrentCulture)){
            csv.Read();
            csv.ReadHeader();

            while(csv.Read()){
                Emission record = new Emission();
                float lat = csv.GetField<float>("Latitude");
                float lon = csv.GetField<float>("Longitude");
                record.C2H6 = csv.GetField<float>("C2H6");
                record.Ch4 = csv.GetField<float>("Ch4");
                record.TimeStamp = csv.GetField<DateTime>("TimeStamp").ToUniversalTime();
                record.Point = new Point(new Coordinate(lon, lat)) { SRID = 4326 };
                records.Add(record);
            }
        }
        return records;
    }
}