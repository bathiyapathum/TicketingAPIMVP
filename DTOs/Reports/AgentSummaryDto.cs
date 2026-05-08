namespace FlightBookingApi.DTOs.Reports;

public class AgentSummaryDto
{
    public int TotalFlights { get; set; }
    public int TotalBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal OccupancyRate { get; set; }
}
