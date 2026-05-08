using FlightBookingApi.Models;

namespace FlightBookingApi.Repositories.Interfaces;

public interface IFlightRepository
{
    Task<List<Flight>> GetFlightsAsync(string? origin, string? destination, DateTime? date);
    Task<Flight?> GetByIdAsync(int flightId);
    Task<int> CountAsync();
}
