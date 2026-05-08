using FlightBookingApi.DTOs.Reservations;

namespace FlightBookingApi.Services.Interfaces;

public interface IReservationService
{
    Task<(bool Success, string Message, ReservationDto? Data)> CreateAsync(CreateReservationRequestDto request);
    Task<ReservationDto?> GetByIdAsync(int reservationId);
    Task<(bool Success, string Message)> CancelAsync(int reservationId);
}
