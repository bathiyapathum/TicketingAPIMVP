namespace FlightBookingApi.Models;

public class Route
{
    public int RouteId { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;

    public ICollection<Flight> Flights { get; set; } = new List<Flight>();
}
