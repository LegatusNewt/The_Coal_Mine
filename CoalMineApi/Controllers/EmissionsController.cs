using Microsoft.AspNetCore.Mvc;
using CoalMineApi.Entities;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace CoalMineApi.Controllers;

[ApiController]
[Route("[controller]")]
public class EmissionsController : ControllerBase
{
    //If you wanted to return the data as wkt
    public class EmissionDto
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
        var GeoUtils = new GeoUtils();
        List<Emission> emissionsData = _context.Emissions.ToList();
        var geoJson = GeoUtils.ConvertToGeoJson<Emission>(emissionsData);
        return Ok(geoJson);
    }

    [HttpGet("data", Name = "GetEmissionsData")]
    public IActionResult GetEmissionsData()
    {
        List<Emission> emissionsData = _context.Emissions.ToList();
        var emissionsDto = emissionsData.Select(e => new EmissionDto
        {
            Id = e.Id,
            CH4 = e.CH4,
            C2H6 = e.C2H6,
            TimeStamp = e.TimeStamp,
            WKT = e.Point.AsText()
        }).ToList();
        return Ok(emissionsDto);
    }
}
