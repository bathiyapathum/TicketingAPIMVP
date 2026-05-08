namespace FlightBookingApi.Models;

public class FlightSeatStatus
{
    public int Id { get; set; }
    public int FlightId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public bool IsReserved { get; set; }
    public int? ReservationId { get; set; }

    public Flight? Flight { get; set; }
    public Reservation? Reservation { get; set; }
}
