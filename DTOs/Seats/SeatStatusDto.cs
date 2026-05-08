namespace FlightBookingApi.DTOs.Seats;

public class SeatStatusDto
{
    public string SeatNumber { get; set; } = string.Empty;
    public bool IsReserved { get; set; }
    public int RowNumber { get; set; }
    public string ColumnLetter { get; set; } = string.Empty;
}
