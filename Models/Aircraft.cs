namespace FlightBookingApi.Models;

public class Aircraft
{
    public int AircraftId { get; set; }
    public string AircraftName { get; set; } = string.Empty;
    public int TotalSeats { get; set; }

    public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    public ICollection<Flight> Flights { get; set; } = new List<Flight>();
}
