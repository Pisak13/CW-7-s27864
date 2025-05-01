using CW_7_s27864.Exceptions;
using CW_7_s27864.Services;
using Microsoft.AspNetCore.Mvc;


namespace CW_7_s27864.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController(IDbService dbService) : ControllerBase
{
    [HttpGet]
    [Route("{id}/trips")]
    public async Task<ActionResult> GetTripsByClient([FromRoute] int id)
    {
        try
        {
            return Ok(await dbService.GetTripsByClient(id));
        }
        catch (NotFoundException e)
        {
            return NotFound(e);
        }
    }
}