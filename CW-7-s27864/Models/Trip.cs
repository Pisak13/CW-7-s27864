namespace CW_7_s27864.Models;

public class Trip
{
    public int IdTrip { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MaxPeople { get; set; }
    public string Country { get; set; }
    
}