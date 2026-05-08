namespace FlightBookingApi.DTOs.Reports;

public class RevenueReportDto
{
    public string Route { get; set; } = string.Empty;
    public string FlightCode { get; set; } = string.Empty;
    public int TotalBookings { get; set; }
    public decimal TotalRevenue { get; set; }
}
