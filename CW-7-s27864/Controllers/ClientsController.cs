using CW_7_s27864.DTOs;
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
    [HttpPost]
    public async Task<ActionResult>CreateClient([FromBody]CreateClientDTO client)
    {
        var result=await dbService.CreateClient(client);
        return Ok("Client Created");
    }

    [HttpPut]
    [Route("{id}/trips/{tripId}")]
    public async Task<ActionResult> AddClientToTrip([FromRoute] int id, [FromRoute] int tripId)
    {
        try
        {
         var result=await dbService.AddClientToTrip(id,tripId);
         return Ok("Client added to trip");
        }catch(NotFoundException e)
        {
            return NotFound(e);
        }
    }
    [HttpDelete]
    [Route("{id}/trips/{tripId}")]
    public async Task<ActionResult> RemoveReservation([FromRoute] int id, [FromRoute] int tripId)
    {
        try
        {
            await dbService.RemoveReservation(id,tripId);
            return Ok("Reservation removed");
        }catch(NotFoundException e)
        {
            return NotFound(e);
        }
    }
 
    
}