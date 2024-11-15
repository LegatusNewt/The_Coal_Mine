using Microsoft.AspNetCore.Mvc;
using CoalMineApi.Entities;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System.Collections.Generic;
using System.Linq;

namespace CoalMineApi.Controllers;

[ApiController]
[Route("[controller]")]
public class EmissionsController : ControllerBase
{
    //If you wanted to return the data as wkt
    private class EmissionDto
    {
        public int Id { get; set; }
        public float? CH4 { get; set; }
        public float? C2H6 { get; set; }
        public DateTime TimeStamp { get; set; }
        public string WKT { get; set; }
    }
    
    private readonly EmissionsDBContext _context;

    public EmissionsController(EmissionsDBContext context)
    {
        _context = context;
    }

    [HttpGet("layer", Name = "GetEmissionsLayer")]
    public IActionResult GetEmissionsLayer()
    {
        List<Emission> emissionsData = _context.Emissions.ToList();
        var geoJson = ConvertToGeoJson(emissionsData);
        return Ok(geoJson);
    }

    [HttpGet("data", Name = "GetEmissionsData")]
    public IActionResult GetEmissionsData()
    {
        List<Emission> emissionsData = _context.Emissions.ToList();
        var emissionsDto = emissionsData.Select(e => new EmissionDto
        {
            Id = e.Id,
            CH4 = e.Ch4,
            C2H6 = e.C2H6,
            TimeStamp = e.TimeStamp,
            WKT = e.Point.AsText()
        }).ToList();
        return Ok(emissionsDto);
    }


    private string ConvertToGeoJson(List<Emission> emissions)
    {
        var features = emissions.Select(emission => new Feature(
            emission.Point,
            new AttributesTable
            {
                { "Id", emission.Id },                
                { "CH4", emission.Ch4 },
                { "C2H6", emission.C2H6 },
                { "TimeStamp", emission.TimeStamp }
            }
        )).ToList();

        var featureCollection = new FeatureCollection(features);
        var writer = new GeoJsonWriter();
        return writer.Write(featureCollection);
    }

}
