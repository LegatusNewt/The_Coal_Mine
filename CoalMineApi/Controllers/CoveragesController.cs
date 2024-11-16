using Microsoft.AspNetCore.Mvc;
using CoalMineApi.Entities;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Utils;
using NetTopologySuite.Operation.Union;

namespace CoalMineApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CoveragesController : ControllerBase
{
    //If you wanted to return the data as wkt
    public class CoverageDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int BufferSize { get; set; }
        public string WKT { get; set; }
    }

    public class CoveragePostBody
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int BufferSize { get; set; }
        public string Feature { get; set; }
    }

    public class CoverageBulkBody
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int BufferSize { get; set; }
        public string FeatureCollection { get; set; }
    }

    private readonly EmissionsDBContext _context;

    public CoveragesController(EmissionsDBContext context)
    {
        _context = context;
    }

    [HttpGet("layer", Name = "GetCoveragesLayer")]
    public IActionResult GetCoveragesLayer()
    {
        var GeoUtils = new GeoUtils();
        List<Coverage> coverageData = _context.Coverages.ToList();
        var geoJson = GeoUtils.ConvertToGeoJson<Coverage>(coverageData);
        return Ok(geoJson);
    }

    [HttpGet("data", Name = "GetCoveragesData")]
    public IActionResult GetCoveragesData()
    {
        List<Coverage> coveragesData = _context.Coverages.ToList();
        var coverageDto = coveragesData.Select(e => new CoverageDTO
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            BufferSize = e.BufferSize,
            WKT = e.Geometry.AsText()
        }).ToList();
        return Ok(coverageDto);
    }

    [HttpPost("data", Name = "PostCoverageData")]
    public IActionResult PostCoverageData([FromBody] CoveragePostBody bodyCoverage)
    {
        if (bodyCoverage == null)
        {
            return BadRequest("bodyCoverage is required");
        }
        // From body is probably going to recieve a geojson?
        // Convert Geojson to polygon
        var geoJsonReader = new GeoJsonReader();
        Feature geo;
        try
        {
            geo = geoJsonReader.Read<Feature>(bodyCoverage.Feature);
        }
        catch (ParseException ex)
        {
            return BadRequest($"Invalid GeoJSON: {ex.Message}");
        }
        /** 
            TODO: There is probably a way to create a hook / helper 
            that will automatically convert the geojson to geometry for the entity
        **/

        var coverage = new Coverage();
        coverage.Name = bodyCoverage.Name;
        if(bodyCoverage.Description != null) {
            coverage.Description = bodyCoverage.Description;
        }

        coverage.BufferSize = bodyCoverage.BufferSize;
        coverage.Geometry = geo.Geometry;
        _context.Coverages.Add(coverage);
        _context.SaveChanges();

        // Return the created coverage record
        return CreatedAtRoute("GetCoveragesData", new { id = coverage.Id }, coverage);
    }

    [HttpGet("data/bulk", Name = "GetCoverageBulkData")]
    public IActionResult GetCoverageBulkData()
    {
        return Ok("Not implemented");
    }

    [HttpPost("data/bulk", Name = "PostCoverageBulkData")]
    public IActionResult PostCoverageBulkData([FromBody] CoverageBulkBody bodyCoverage)
    {
        if (bodyCoverage == null)
        {
            return BadRequest("bodyCoverage is required");
        }
        // From body is probably going to recieve a geojson?
        // Convert Geojson to polygon
        FeatureCollection featureCollection = new FeatureCollection();
        var geoJsonReader = new GeoJsonReader();
        try
        {
            featureCollection = geoJsonReader.Read<FeatureCollection>(bodyCoverage.FeatureCollection);
        }
        catch (ParseException ex)
        {
            return BadRequest($"Invalid GeoJSON: {ex.Message}");
        }

        var polygons = new List<Geometry>();
        foreach (var feature in featureCollection)
        {
            if (feature.Geometry is Polygon)
            {
                polygons.Add(feature.Geometry);
            }
            else
            {
                return BadRequest("All geometries in the feature collection must be polygons");
            }
        }

        if (polygons.Count == 0)
        {
            return BadRequest("No polygons found in the feature collection");
        }

        var coverage = new Coverage();

        coverage.Name = bodyCoverage.Name;
        if(bodyCoverage.Description != null) {
            coverage.Description = bodyCoverage.Description;
        }
        coverage.BufferSize = bodyCoverage.BufferSize;

        // Attempt to Overlap the polygons
        CascadedPolygonUnion union = new CascadedPolygonUnion(polygons);
        Geometry unionedPolygons = union.Union();

        coverage.Geometry = unionedPolygons;

        _context.Coverages.Add(coverage);
        _context.SaveChanges();

        // Return the created coverage record
        return CreatedAtRoute("GetCoveragesData", new { id = coverage.Id }, coverage);
    }
}
