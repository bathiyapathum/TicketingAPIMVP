namespace FlightBookingApi.DTOs.Flights;

public class FlightListItemDto
{
    public int FlightId { get; set; }
    public string FlightCode { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public int DurationMinutes { get; set; }
    public decimal BasePrice { get; set; }
}
