namespace FlightBookingApi.DTOs.Reservations;

public class ReservationDto
{
    public int ReservationId { get; set; }
    public int FlightId { get; set; }
    public string FlightCode { get; set; } = string.Empty;
    public string PassengerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PassportNo { get; set; } = string.Empty;
    public string SeatNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string BookingStatus { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
}
