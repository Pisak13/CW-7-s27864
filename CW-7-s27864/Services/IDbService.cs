using CW_7_s27864.DTOs;
using CW_7_s27864.Exceptions;
using Microsoft.Data.SqlClient;

namespace CW_7_s27864.Services;

public interface IDbService
{
    public Task<IEnumerable<TripWithCountryDTO>> GetTripsAsync();
    public Task<IEnumerable<TripsClientDTO>> GetTripsByClient(int id);
}

public class DBService(IConfiguration configuration) : IDbService
{
    private readonly string? _conString = configuration.GetConnectionString("DB");

    public async Task<IEnumerable<TripWithCountryDTO>> GetTripsAsync()
    {
        await using var con = new SqlConnection(_conString);
        await using var com = new SqlCommand();
        com.Connection = con;
        com.CommandText =
            @"SELECT  Trip.IdTrip,Trip.Name,Trip.Description,Trip.DateFrom,Trip.DateTo,Trip.MaxPeople,C.Name AS CountryName FROM Trip 
                          INNER JOIN s27864.Country_Trip CT on Trip.IdTrip = CT.IdTrip 
                          INNER JOIN s27864.Country C on C.IdCountry = CT.IdCountry";
        await con.OpenAsync();
        SqlDataReader reader = await com.ExecuteReaderAsync();
        var trips = new List<TripWithCountryDTO>();
        while (await reader.ReadAsync())
        {
            var tripss = new TripWithCountryDTO()
            {
                IdTrip = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                StartDate = reader.GetDateTime(3),
                EndDate = reader.GetDateTime(4),
                MaxPeople = reader.GetInt32(5),
                Country = reader.GetString(6)
            };
            trips.Add(tripss);
        }

        return trips;
    }


    public async Task<IEnumerable<TripsClientDTO>> GetTripsByClient(int id)
    {
        await using var con = new SqlConnection(_conString); 
        await con.OpenAsync();
        const string query1 = "SELECT 1 FROM CLIENT WHERE IdClient = @id";
        await using (var com1 = new SqlCommand(query1, con))
        {
            com1.Parameters.AddWithValue("@id", id);

            using (var reader1 = await com1.ExecuteReaderAsync())
            {
                if (!reader1.HasRows)
                {
                    throw new NotFoundException($"Client with id: {id} does not exist");
                }

            }

            await con.CloseAsync();
        }
        const string query2 =
            @"SELECT Trip.IdTrip,Trip.Name,Trip.Description,Trip.DateFrom,Trip.DateTo,Trip.MaxPeople,CT.RegisteredAt,CT.PaymentDate FROM Trip 
                                                           INNER JOIN s27864.Client_Trip CT on Trip.IdTrip = CT.IdTrip
                                                           WHERE CT.IdClient = @id";
        await using var com2 = new SqlCommand(query2, con);
        com2.Parameters.AddWithValue("@id", id);
        await con.OpenAsync();
        using var reader2 = await com2.ExecuteReaderAsync();

        if (!reader2.HasRows)
        {
            throw new NotFoundException($"Client with id: {id} does have any trips");
        }

        var trips = new List<TripsClientDTO>();
        while (await reader2.ReadAsync())
        {
           trips.Add(new TripsClientDTO
            {
                IdTrip = reader2.GetInt32(0),
                Name = reader2.GetString(1),
                Description = reader2.GetString(2),
                StartDate = reader2.GetDateTime(3),
                EndDate = reader2.GetDateTime(4),
                MaxPeople = reader2.GetInt32(5),
                RegisteredAt = reader2.GetInt32(6),
                PaymentDate = reader2.GetInt32(7)
            });
            
        }

        return trips;
    }
}