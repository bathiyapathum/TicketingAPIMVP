namespace FlightBookingApi.DTOs.Flights;

public class FlightDetailsDto : FlightListItemDto
{
    public string AircraftName { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
    public int ReservedSeats { get; set; }
    public int AvailableSeats { get; set; }
}
