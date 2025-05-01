namespace CW_7_s27864.DTOs;

public class TripsClientDTO
{
    public int IdTrip { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MaxPeople { get; set; }
    public int RegisteredAt { get; set; }
    public int PaymentDate { get; set; }
}