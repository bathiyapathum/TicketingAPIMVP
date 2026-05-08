using AutoMapper;
using FlightBookingApi.DTOs.Flights;
using FlightBookingApi.Repositories.Interfaces;
using FlightBookingApi.Services.Interfaces;

namespace FlightBookingApi.Services.Implementations;

public class FlightService(IFlightRepository flightRepository, IMapper mapper) : IFlightService
{
    public async Task<List<FlightListItemDto>> GetFlightsAsync(string? origin, string? destination, DateTime? date)
    {
        var flights = await flightRepository.GetFlightsAsync(origin, destination, date);
        return mapper.Map<List<FlightListItemDto>>(flights);
    }

    public async Task<FlightDetailsDto?> GetByIdAsync(int id)
    {
        var flight = await flightRepository.GetByIdAsync(id);
        if (flight is null)
        {
            return null;
        }

        var dto = mapper.Map<FlightDetailsDto>(flight);
        dto.AircraftName = flight.Aircraft?.AircraftName ?? string.Empty;
        dto.TotalSeats = flight.Aircraft?.TotalSeats ?? 0;
        dto.ReservedSeats = flight.SeatStatuses.Count(x => x.IsReserved);
        dto.AvailableSeats = dto.TotalSeats - dto.ReservedSeats;

        return dto;
    }
}
