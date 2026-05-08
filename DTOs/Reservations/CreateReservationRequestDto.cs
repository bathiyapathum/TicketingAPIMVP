using System.ComponentModel.DataAnnotations;

namespace FlightBookingApi.DTOs.Reservations;

public class CreateReservationRequestDto
{
    [Required]
    public int FlightId { get; set; }

    [Required]
    [MaxLength(150)]
    public string PassengerName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string PassportNo { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^[1-2][A-D]$")]
    public string SeatNumber { get; set; } = string.Empty;
}
