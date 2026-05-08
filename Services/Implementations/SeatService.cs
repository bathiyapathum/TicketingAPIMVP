using FlightBookingApi.DTOs.Seats;
using FlightBookingApi.Repositories.Interfaces;
using FlightBookingApi.Services.Interfaces;

namespace FlightBookingApi.Services.Implementations;

public class SeatService(ISeatRepository seatRepository) : ISeatService
{
    public async Task<List<SeatStatusDto>> GetSeatsByFlightAsync(int flightId)
    {
        var seats = await seatRepository.GetSeatStatusByFlightAsync(flightId);
        return seats
            .Select(x => new SeatStatusDto
            {
                SeatNumber = x.Seat.SeatNumber,
                IsReserved = x.Status.IsReserved,
                RowNumber = x.Seat.RowNumber,
                ColumnLetter = x.Seat.ColumnLetter
            })
            .ToList();
    }
}
