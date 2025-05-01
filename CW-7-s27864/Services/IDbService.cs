using System.Text.RegularExpressions;
using CW_7_s27864.DTOs;
using CW_7_s27864.Exceptions;
using CW_7_s27864.Models;
using Microsoft.Data.SqlClient;

namespace CW_7_s27864.Services;

public interface IDbService
{
    public Task<IEnumerable<TripWithCountryDTO>> GetTripsAsync();
    public Task<IEnumerable<TripsClientDTO>> GetTripsByClient(int id);
    public Task<CreateClientDTO> CreateClient(CreateClientDTO client);
    public Task<Client_Trip> AddClientToTrip(int id, int idTrip);
    public Task RemoveReservation(int id, int idTrip);
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

    public async Task<CreateClientDTO> CreateClient(CreateClientDTO client)
    {
        await using var con = new SqlConnection(_conString);
        await con.OpenAsync();

        const string query = @"
        INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel) 
        VALUES (@firstName, @lastName, @email, @phone, @pesel);
        SELECT SCOPE_IDENTITY();";

        await using var com = new SqlCommand(query, con);

        com.Parameters.AddWithValue("@firstName", client.FirstName);
        com.Parameters.AddWithValue("@lastName", client.LastName);
        com.Parameters.AddWithValue("@email", client.Email);
        com.Parameters.AddWithValue("@phone", client.Phone);
        com.Parameters.AddWithValue("@pesel", client.Pesel);

        var id = Convert.ToInt32(await com.ExecuteScalarAsync());

        return new CreateClientDTO
        {
            IdClient = id,
            FirstName = client.FirstName,
            LastName = client.LastName,
            Email = client.Email,
            Phone = client.Phone,
            Pesel = client.Pesel
        };
    }

    public async Task<Client_Trip> AddClientToTrip(int id, int idTrip)
    {
        await using var con = new SqlConnection(_conString);
        await con.OpenAsync();
        const string query1 = "SELECT 1 FROM CLIENT WHERE IdClient=@id";
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

            
        }
        const string query2="SELECT 1 FROM Trip WHERE IdTrip=@idTrip";
        await using (var com2 = new SqlCommand(query2, con))
        {
            com2.Parameters.AddWithValue("@idTrip", idTrip);
            using (var reader2 = await com2.ExecuteReaderAsync())
            {
                if (!reader2.HasRows)
                {
                    throw new NotFoundException($"Trip with id: {idTrip} does not exist");
                }
            }
        }

        const string query3 =
            @"SELECT (SELECT COUNT(*)FROM Client_Trip WHERE IdTrip=@idTrip) AS CURRENTCount,
(SELECT MaxPeople from Trip WHERE IdTrip=@idTrip) AS MAX";
        await using (var com3 = new SqlCommand(query3, con))
        {
            com3.Parameters.AddWithValue("@idTrip", idTrip);
            using (var reader3 = await com3.ExecuteReaderAsync())
            {
                if (await reader3.ReadAsync())
                {
                    int currentCount = reader3.GetInt32(0);
                    int max = reader3.GetInt32(1);
                    if (currentCount >= max)
                        throw new NotFoundException($"Trip with id: {idTrip} is full");
                }
            }
        }
        const string query4 =
            @"INSERT INTO Client_Trip (IdClient, IdTrip,RegisteredAt, PaymentDate) VALUES (@id, @idTrip, @registeredAt, @paymentDate)";
        int today=Convert.ToInt32(DateTime.Today.ToString("yyyyMMdd"));
        await using (var com4 = new SqlCommand(query4, con))
        {
            com4.Parameters.AddWithValue("@id", id);
            com4.Parameters.AddWithValue("@idTrip",idTrip);
            com4.Parameters.AddWithValue("@registeredAt",today );
            com4.Parameters.AddWithValue("@paymentDate", today);
            await com4.ExecuteNonQueryAsync();
        }
        

        return new Client_Trip()
        {
            IdClient = id,
            IdTrip = idTrip,
            RegisteredAt = today,
            PaymentDate = today
        };
        

    }

    public async Task RemoveReservation(int id, int idTrip)
    {
        await using var con=new SqlConnection(_conString);
        await con.OpenAsync();
        const string query1 = "SELECT 1 FROM Client_Trip WHERE IdClient=@id AND IdTrip=@idTrip";
        await using (var com1 = new SqlCommand(query1, con))
        {
            com1.Parameters.AddWithValue("@id", id);
            com1.Parameters.AddWithValue("@idTrip", idTrip);
            using (var reader1 = await com1.ExecuteReaderAsync())
            {
                if (!reader1.HasRows)
                {
                    throw new NotFoundException($"This reservation does not exist");
                }
            }
            
        }
        const string query2 = "DELETE FROM Client_Trip WHERE IdClient=@id";
        await using var com2=new SqlCommand(query2, con);
        com2.Parameters.AddWithValue("@id", id);
        await com2.ExecuteNonQueryAsync();
        
    }



}