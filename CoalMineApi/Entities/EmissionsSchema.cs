using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

[Table("Emissions", Schema = "CoalMine")]
public class Emission
{
    public int? Id { get; set; }
    public float? Ch4 { get; set; }
    public float? C2H6 { get; set; }
    public Point? Point { get; set; } // GeoJSON string -> change to use geometry types
    public DateTime TimeStamp { get; set; }
    private float Latitude { get; set; }
    private float Longitude { get; set; }
}

[Table("Coverages", Schema = "CoalMine")]
public class Coverage
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Polygon? Polygon { get; set; } // GeoJSON string -> change to use geometry types
}