using CW_7_s27864.Services;
using Microsoft.AspNetCore.Mvc;
namespace CW_7_s27864.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController(IDbService dbService) : ControllerBase
{
    /// <summary>
    /// Zwraca wszystkie wycieczki i informacje o nich
    /// </summary>
    /// <returns>Liste wszystkich wycieczek</returns>
    [HttpGet]
    public async Task<ActionResult> GetAllTrips()
    {
        return Ok(await dbService.GetTripsAsync());
    }
}