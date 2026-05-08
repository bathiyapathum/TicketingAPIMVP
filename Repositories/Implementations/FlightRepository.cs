using FlightBookingApi.Data;
using FlightBookingApi.Models;
using FlightBookingApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlightBookingApi.Repositories.Implementations;

public class FlightRepository(FlightBookingDbContext dbContext) : IFlightRepository
{
    public async Task<List<Flight>> GetFlightsAsync(string? origin, string? destination, DateTime? date)
    {
        var query = dbContext.Flights
            .Include(x => x.Route)
            .Include(x => x.Aircraft)
            .Include(x => x.SeatStatuses)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(origin))
        {
            query = query.Where(x => x.Route != null && x.Route.Origin == origin);
        }

        if (!string.IsNullOrWhiteSpace(destination))
        {
            query = query.Where(x => x.Route != null && x.Route.Destination == destination);
        }

        if (date.HasValue)
        {
            var targetDate = date.Value.Date;
            query = query.Where(x => x.DepartureTime.Date == targetDate);
        }

        return await query.OrderBy(x => x.DepartureTime).ToListAsync();
    }

    public Task<Flight?> GetByIdAsync(int flightId)
    {
        return dbContext.Flights
            .Include(x => x.Route)
            .Include(x => x.Aircraft)
            .Include(x => x.SeatStatuses)
            .FirstOrDefaultAsync(x => x.FlightId == flightId);
    }

    public Task<int> CountAsync() => dbContext.Flights.CountAsync();
}
