using CW_7_s27864.DTOs;
using CW_7_s27864.Exceptions;
using CW_7_s27864.Services;
using Microsoft.AspNetCore.Mvc;


namespace CW_7_s27864.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController(IDbService dbService) : ControllerBase
{
    /// <summary>
    /// Zwraca wszystkie wycieczki, na które zapisany jest klient.
    /// </summary>
    /// <param name="id">Identyfikator klienta</param>
    /// <returns>Lista wycieczek klienta</returns>
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
    /// <summary>
    /// Tworzy nowego klienta
    /// </summary>
    /// <param name="client">Dane nowego klienta</param>
    /// <returns>Komunikat o powodzeniu operacji</returns>
    [HttpPost]
    public async Task<ActionResult>CreateClient([FromBody]CreateClientDTO client)
    {
        await dbService.CreateClient(client);
        return Ok("Client Created");
    }
/// <summary>
/// Dodaje klienta do wskazanej wycieczki
/// </summary>
/// <param name="id">Identyfikator klienta</param>
/// <param name="tripId">Identyfikator wycieczki</param>
/// <returns>komunikat o powodzeniu operacji</returns>
    [HttpPut]
    [Route("{id}/trips/{tripId}")]
    public async Task<ActionResult> AddClientToTrip([FromRoute] int id, [FromRoute] int tripId)
    {
        try
        {
         await dbService.AddClientToTrip(id,tripId);
         return Ok("Client added to trip");
        }catch(NotFoundException e)
        {
            return NotFound(e);
        }
    }
/// <summary>
/// Usuwa rezerwacje klienta z wycieczki
/// </summary>
/// <param name="id">Indentyfikator klienta</param>
/// <param name="tripId">Identyfikator wycieczki</param>
/// <returns>Komunikat o powodzeniu operacji</returns>
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