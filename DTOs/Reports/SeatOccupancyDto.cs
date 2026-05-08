namespace FlightBookingApi.DTOs.Reports;

public class SeatOccupancyDto
{
    public int FlightId { get; set; }
    public string FlightCode { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
    public int ReservedSeats { get; set; }
    public decimal OccupancyRate { get; set; }
}
