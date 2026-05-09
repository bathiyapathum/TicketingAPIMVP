using FlightBookingApi.Common;
using FlightBookingApi.Data;
using FlightBookingApi.DTOs.Reports;
using FlightBookingApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlightBookingApi.Repositories.Implementations;

public class ReportRepository(FlightBookingDbContext dbContext) : IReportRepository
{
    public async Task<List<RevenueReportDto>> GetRevenueAsync(DateTime? from, DateTime? to)
    {
        var query = dbContext.Reservations
            .Where(x => x.BookingStatus == BookingStatus.Booked)
            .AsQueryable();

        if (from.HasValue)
        {
            query = query.Where(x => x.BookingDate.Date >= from.Value.Date);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.BookingDate.Date <= to.Value.Date);
        }

        return await query
            .GroupBy(x => new
            {
                Origin = x.Flight!.Route!.Origin,
                Destination = x.Flight.Route.Destination,
                FlightCode = x.Flight.FlightCode
            })
            .Select(g => new RevenueReportDto
            {
                Route = g.Key.Origin + " -> " + g.Key.Destination,
                FlightCode = g.Key.FlightCode,
                TotalBookings = g.Count(),
                TotalRevenue = g.Sum(x => x.TotalAmount)
            })
            .OrderBy(x => x.Route)
            .ThenBy(x => x.FlightCode)
            .ToListAsync();
    }

    public async Task<List<BookingsByRouteDto>> GetBookingsByRouteAsync()
    {
        return await dbContext.Reservations
            .Where(x => x.BookingStatus == BookingStatus.Booked)
            .GroupBy(x => new
            {
                Origin = x.Flight!.Route!.Origin,
                Destination = x.Flight.Route.Destination
            })
            .Select(g => new BookingsByRouteDto
            {
                Route = g.Key.Origin + " -> " + g.Key.Destination,
                BookingCount = g.Count()
            })
            .OrderByDescending(x => x.BookingCount)
            .ToListAsync();
    }

    public async Task<SeatOccupancyDto?> GetSeatOccupancyAsync(int flightId)
    {
        var flight = await dbContext.Flights.AsNoTracking().FirstOrDefaultAsync(x => x.FlightId == flightId);
        if (flight is null)
        {
            return null;
        }

        var totalSeats = await dbContext.Seats.CountAsync(x => x.AircraftId == flight.AircraftId);
        var reservedSeats = await dbContext.FlightSeatStatuses.CountAsync(x => x.FlightId == flightId && x.IsReserved);

        return new SeatOccupancyDto
        {
            FlightId = flight.FlightId,
            FlightCode = flight.FlightCode,
            TotalSeats = totalSeats,
            ReservedSeats = reservedSeats,
            OccupancyRate = totalSeats == 0 ? 0 : Math.Round((decimal)reservedSeats / totalSeats * 100m, 2)
        };
    }

    public async Task<List<DailyBookingTrendDto>> GetDailyBookingsAsync(DateTime? from, DateTime? to)
    {
        var query = dbContext.Reservations
            .Where(x => x.BookingStatus == BookingStatus.Booked)
            .AsQueryable();

        if (from.HasValue)
        {
            query = query.Where(x => x.BookingDate.Date >= from.Value.Date);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.BookingDate.Date <= to.Value.Date);
        }

        return await query
            .GroupBy(x => x.BookingDate.Date)
            .Select(g => new DailyBookingTrendDto
            {
                Date = g.Key,
                BookingCount = g.Count(),
                Revenue = g.Sum(x => x.TotalAmount)
            })
            .OrderBy(x => x.Date)
            .ToListAsync();
    }

    public async Task<decimal> GetOverallOccupancyRateAsync()
    {
        var total = await dbContext.FlightSeatStatuses.CountAsync();
        if (total == 0)
        {
            return 0m;
        }

        var reserved = await dbContext.FlightSeatStatuses.CountAsync(x => x.IsReserved);
        return Math.Round((decimal)reserved / total * 100m, 2);
    }
}
