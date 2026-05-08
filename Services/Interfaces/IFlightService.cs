using FlightBookingApi.DTOs.Flights;

namespace FlightBookingApi.Services.Interfaces;

public interface IFlightService
{
    Task<List<FlightListItemDto>> GetFlightsAsync(string? origin, string? destination, DateTime? date);
    Task<FlightDetailsDto?> GetByIdAsync(int id);
}
