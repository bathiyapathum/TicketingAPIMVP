namespace FlightBookingApi.DTOs.Reports;

public class DailyBookingTrendDto
{
    public DateTime Date { get; set; }
    public int BookingCount { get; set; }
    public decimal Revenue { get; set; }
}
