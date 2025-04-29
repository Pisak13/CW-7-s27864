using CW_7_s27864.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace CW_7_s27864.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ClientsController:ControllerBase
{
    [HttpGet]
    [Route("{id}/trips")]
    public async Task<ActionResult> GetTripsByClient(int id, CancellationToken cancellationToken)
    {
        await using var con = new SqlConnection(TripsController.conString);
        await using var com = new SqlCommand();
        com.Connection = con;
        com.CommandText = @"SELECT Trip.IdTrip,Trip.Name,Trip.Description,Trip.DateFrom,Trip.DateTo,Trip.MaxPeople,CT.RegisteredAt,CT.PaymentDate FROM Trip 
                                                           INNER JOIN s27864.Client_Trip CT on Trip.IdTrip = CT.IdTrip
                                                           WHERE CT.IdClient = @id";
        com.Parameters.AddWithValue("@id", id);
        
        await con.OpenAsync(cancellationToken);
        SqlDataReader reader = await com.ExecuteReaderAsync(cancellationToken);
        var tripsClient = new List<TripsClient>();
        while (await reader.ReadAsync(cancellationToken))
        {
            int idtrip = (int)reader["IdTrip"];
            string name = (string)reader["Name"];
            string description = (string)reader["Description"];
            DateTime startDate = (DateTime)reader["DateFrom"];
            DateTime endDate = (DateTime)reader["DateTo"];
            int maxPeople = (int)reader["MaxPeople"];
            int registeredAt = (int)reader["RegisteredAt"];
            int paymentDate = (int)reader["PaymentDate"];
            TripsClient tc = new TripsClient()
            {
                IdTrip = idtrip,
                Name = name,
                Description = description,
                StartDate = startDate,
                EndDate = endDate,
                MaxPeople = maxPeople,
                RegisteredAt = registeredAt,
                PaymentDate = paymentDate
            };
            tripsClient.Add(tc);
        }
        
        com.DisposeAsync();
        
       
        return Ok(tripsClient);
    }
}