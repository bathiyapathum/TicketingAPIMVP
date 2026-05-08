using FlightBookingApi.Data;
using FlightBookingApi.Models;
using FlightBookingApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlightBookingApi.Repositories.Implementations;

public class SeatRepository(FlightBookingDbContext dbContext) : ISeatRepository
{
    public async Task<List<(Seat Seat, FlightSeatStatus Status)>> GetSeatStatusByFlightAsync(int flightId)
    {
        var flight = await dbContext.Flights.AsNoTracking().FirstOrDefaultAsync(x => x.FlightId == flightId);
        if (flight is null)
        {
            return [];
        }

        var seats = await dbContext.Seats
            .AsNoTracking()
            .Where(x => x.AircraftId == flight.AircraftId)
            .OrderBy(x => x.RowNumber)
            .ThenBy(x => x.ColumnLetter)
            .ToListAsync();

        var statuses = await dbContext.FlightSeatStatuses
            .AsNoTracking()
            .Where(x => x.FlightId == flightId)
            .ToDictionaryAsync(x => x.SeatNumber, x => x);

        return seats
            .Where(s => statuses.ContainsKey(s.SeatNumber))
            .Select(s => (s, statuses[s.SeatNumber]))
            .ToList();
    }

    public Task<FlightSeatStatus?> GetSeatStatusAsync(int flightId, string seatNumber)
    {
        return dbContext.FlightSeatStatuses
            .FirstOrDefaultAsync(x => x.FlightId == flightId && x.SeatNumber == seatNumber);
    }

    public Task<int> GetReservedSeatCountAsync(int flightId)
    {
        return dbContext.FlightSeatStatuses
            .CountAsync(x => x.FlightId == flightId && x.IsReserved);
    }
}
