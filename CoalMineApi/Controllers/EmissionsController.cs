using Microsoft.AspNetCore.Mvc;
using CsvHelper;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using NetTopologySuite.Geometries;

namespace CoalMineApi.Controllers;

[ApiController]
[Route("[controller]")]
public class EmissionsController : ControllerBase
{
    private readonly ILogger<EmissionsController> _logger;
    private readonly EmissionsDBContext _context;

    public EmissionsController(ILogger<EmissionsController> logger, EmissionsDBContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet(Name = "GetEmissionsData")]
    public IActionResult GetEmissionsData()
    {
        List<Emission> emissionsData = _context.Emissions.ToList();
        return Ok(emissionsData);
    }

}
