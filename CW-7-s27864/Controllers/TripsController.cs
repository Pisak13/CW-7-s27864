using CW_7_s27864.Services;
using Microsoft.AspNetCore.Mvc;
namespace CW_7_s27864.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController(IDbService dbService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetAllTrips()
    {
        return Ok(await dbService.GetTripsAsync());
    }
}