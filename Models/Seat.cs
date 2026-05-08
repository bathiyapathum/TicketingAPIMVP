namespace FlightBookingApi.Models;

public class Seat
{
    public int SeatId { get; set; }
    public int AircraftId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public int RowNumber { get; set; }
    public string ColumnLetter { get; set; } = string.Empty;

    public Aircraft? Aircraft { get; set; }
}
