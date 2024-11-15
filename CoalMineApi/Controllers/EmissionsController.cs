using Microsoft.AspNetCore.Mvc;
using CoalMineApi.Entities;

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

    public EmissionsController(EmissionsDBContext context) // For test suite
    {
        _context = context;
    }

    [HttpGet(Name = "GetEmissionsData")]
    public IActionResult GetEmissionsData()
    {
        List<Emission> emissionsData = _context.Emissions.ToList();
        return Ok(emissionsData);
    }

}
