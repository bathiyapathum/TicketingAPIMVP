using FlightBookingApi.Common;

namespace FlightBookingApi.Models;

public class Reservation
{
    public int ReservationId { get; set; }
    public int FlightId { get; set; }
    public string PassengerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PassportNo { get; set; } = string.Empty;
    public string SeatNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public BookingStatus BookingStatus { get; set; }
    public DateTime BookingDate { get; set; }

    public Flight? Flight { get; set; }
}
