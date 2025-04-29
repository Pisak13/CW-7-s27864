using CW_7_s27864.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace CW_7_s27864.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    public static string conString =
        "Data Source=DB-MSSQL16.pjwstk.edu.pl,1433;Initial Catalog=2019SBD;Persist Security Info=True;User ID=s27864@pjwstk.edu.pl;Password=Pisak1317;Integrated Security=True;TrustServerCertificate=True;Encrypt=True;";

   
    

    public async Task<ActionResult> GetAllTripsAsync(CancellationToken cancellationToken)
    {
        await using var con = new SqlConnection(conString);
        await using var com = new SqlCommand();
        com.Connection = con;
        com.CommandText = @"SELECT  Trip.IdTrip,Trip.Name,Trip.Description,Trip.DateFrom,Trip.DateTo,Trip.MaxPeople,C.Name AS CountryName FROM Trip 
                          INNER JOIN s27864.Country_Trip CT on Trip.IdTrip = CT.IdTrip 
                          INNER JOIN s27864.Country C on C.IdCountry = CT.IdCountry";
        await con.OpenAsync(cancellationToken);
        SqlDataReader reader = await com.ExecuteReaderAsync(cancellationToken);
        var trips = new List<Trip>();
        while (await reader.ReadAsync(cancellationToken))
        {
            int idTrip = (int)reader["IdTrip"];
            string name = (string)reader["Name"];
            string description = (string)reader["Description"];
            DateTime startDate = (DateTime)reader["DateFrom"];
            DateTime endDate = (DateTime)reader["DateTo"];
            int maxPeople = (int)reader["MaxPeople"];
            string country = (string)reader["CountryName"];
            Trip t = new Trip()
            {
                IdTrip = idTrip,
                Name = name,
                Description = description,
                StartDate = startDate,
                EndDate = endDate,
                MaxPeople = maxPeople,
                Country = country
            };

            trips.Add(t);
        }

        com.DisposeAsync();
        return Ok(trips);
    }
    [HttpGet ( "clients/{id}/trips")]
    public async Task<ActionResult> GetTripsByClient(int id, CancellationToken cancellationToken)
    {
        await using var con = new SqlConnection(conString);
        await using var com = new SqlCommand();
        com.Connection = con;
        
        return Ok();
    }
    
}