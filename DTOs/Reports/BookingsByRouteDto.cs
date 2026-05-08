namespace FlightBookingApi.DTOs.Reports;

public class BookingsByRouteDto
{
    public string Route { get; set; } = string.Empty;
    public int BookingCount { get; set; }
}
