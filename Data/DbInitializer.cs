using FlightBookingApi.Models;
using Microsoft.EntityFrameworkCore;
using RouteEntity = FlightBookingApi.Models.Route;

namespace FlightBookingApi.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(FlightBookingDbContext dbContext, bool applyMigrations)
    {
        if (applyMigrations)
        {
            await dbContext.Database.MigrateAsync();
        }

        if (await dbContext.Routes.AnyAsync())
        {
            return;
        }

        var routes = new List<RouteEntity>
        {
            new() { Origin = "Colombo", Destination = "Dubai" },
            new() { Origin = "Dubai", Destination = "Colombo" },
            new() { Origin = "Colombo", Destination = "Singapore" },
            new() { Origin = "Singapore", Destination = "Colombo" }
        };

        dbContext.Routes.AddRange(routes);
        await dbContext.SaveChangesAsync();

        var aircraft = new Aircraft
        {
            AircraftName = "Airbus A220-MVP",
            TotalSeats = 8
        };

        dbContext.Aircrafts.Add(aircraft);
        await dbContext.SaveChangesAsync();

        var seatNumbers = new[] { "1A", "1B", "1C", "1D", "2A", "2B", "2C", "2D" };
        var seats = seatNumbers
            .Select(sn => new Seat
            {
                AircraftId = aircraft.AircraftId,
                SeatNumber = sn,
                RowNumber = int.Parse(sn[..1]),
                ColumnLetter = sn[1..]
            })
            .ToList();

        dbContext.Seats.AddRange(seats);

        var now = DateTime.UtcNow.Date.AddHours(6);
        var flights = new List<Flight>
        {
            new()
            {
                FlightCode = "FB1001",
                RouteId = routes[0].RouteId,
                AircraftId = aircraft.AircraftId,
                DepartureTime = now.AddDays(1),
                ArrivalTime = now.AddDays(1).AddHours(4),
                DurationMinutes = 240,
                BasePrice = 320m
            },
            new()
            {
                FlightCode = "FB1002",
                RouteId = routes[1].RouteId,
                AircraftId = aircraft.AircraftId,
                DepartureTime = now.AddDays(2),
                ArrivalTime = now.AddDays(2).AddHours(4),
                DurationMinutes = 240,
                BasePrice = 330m
            },
            new()
            {
                FlightCode = "FB1003",
                RouteId = routes[2].RouteId,
                AircraftId = aircraft.AircraftId,
                DepartureTime = now.AddDays(1).AddHours(3),
                ArrivalTime = now.AddDays(1).AddHours(7),
                DurationMinutes = 240,
                BasePrice = 280m
            },
            new()
            {
                FlightCode = "FB1004",
                RouteId = routes[3].RouteId,
                AircraftId = aircraft.AircraftId,
                DepartureTime = now.AddDays(3),
                ArrivalTime = now.AddDays(3).AddHours(4),
                DurationMinutes = 240,
                BasePrice = 290m
            },
            new()
            {
                FlightCode = "FB1005",
                RouteId = routes[0].RouteId,
                AircraftId = aircraft.AircraftId,
                DepartureTime = now.AddDays(4),
                ArrivalTime = now.AddDays(4).AddHours(4),
                DurationMinutes = 240,
                BasePrice = 340m
            }
        };

        dbContext.Flights.AddRange(flights);
        await dbContext.SaveChangesAsync();

        var flightSeatStatuses = new List<FlightSeatStatus>();
        foreach (var flight in flights)
        {
            flightSeatStatuses.AddRange(seatNumbers.Select(seatNumber => new FlightSeatStatus
            {
                FlightId = flight.FlightId,
                SeatNumber = seatNumber,
                IsReserved = false,
                ReservationId = null
            }));
        }

        dbContext.FlightSeatStatuses.AddRange(flightSeatStatuses);
        await dbContext.SaveChangesAsync();
    }
}
