using FlightBookingApi.DTOs.Seats;

namespace FlightBookingApi.Services.Interfaces;

public interface ISeatService
{
    Task<List<SeatStatusDto>> GetSeatsByFlightAsync(int flightId);
}
