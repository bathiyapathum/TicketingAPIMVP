using FlightBookingApi.Models;

namespace FlightBookingApi.Repositories.Interfaces;

public interface ISeatRepository
{
    Task<List<(Seat Seat, FlightSeatStatus Status)>> GetSeatStatusByFlightAsync(int flightId);
    Task<FlightSeatStatus?> GetSeatStatusAsync(int flightId, string seatNumber);
    Task<int> GetReservedSeatCountAsync(int flightId);
}
