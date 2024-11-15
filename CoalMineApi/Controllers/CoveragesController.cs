using Microsoft.AspNetCore.Mvc;
using CoalMineApi.Entities;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace CoalMineApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CoveragesController : ControllerBase
{
    //If you wanted to return the data as wkt
    public class CoverageDTO
    {
        public int Id { get; set; }
        public string ?Name { get; set; }
        public string ?Description { get; set; }
        public int BufferSize { get; set; }
        public string WKT { get; set; }
    }

    public class CoveragePostBody
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int BufferSize { get; set; }
        public string GeoJSON { get; set; }
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
    public IActionResult GetEmissionsData()
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
        // From body is probably going to recieve a geojson?
        // Convert Geojson to polygon
        var geo = new GeoJsonReader().Read<Polygon>(bodyCoverage.GeoJSON);
        var coverage = new Coverage();
        coverage.Name = bodyCoverage.Name;
        coverage.Description = bodyCoverage.Description;
        coverage.BufferSize = bodyCoverage.BufferSize;
        coverage.Geometry = geo;
        _context.Coverages.Add(coverage);
        _context.SaveChanges();
        return CreatedAtRoute("GetCoveragesData", new { id = coverage.Id }, coverage);
    }
}
