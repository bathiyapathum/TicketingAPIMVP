using FlightBookingApi.Data;
using FlightBookingApi.Common;
using FlightBookingApi.Models;
using FlightBookingApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlightBookingApi.Repositories.Implementations;

public class ReservationRepository(FlightBookingDbContext dbContext) : IReservationRepository
{
    public Task<Reservation?> GetByIdAsync(int reservationId)
    {
        return dbContext.Reservations
            .Include(x => x.Flight)
            .FirstOrDefaultAsync(x => x.ReservationId == reservationId);
    }

    public async Task<Reservation> AddAsync(Reservation reservation)
    {
        dbContext.Reservations.Add(reservation);
        await dbContext.SaveChangesAsync();
        return reservation;
    }

    public async Task UpdateAsync(Reservation reservation)
    {
        dbContext.Reservations.Update(reservation);
        await dbContext.SaveChangesAsync();
    }

    public Task<int> CountBookedAsync()
    {
        return dbContext.Reservations.CountAsync(x => x.BookingStatus == BookingStatus.Booked);
    }

    public Task<decimal> SumRevenueAsync()
    {
        return dbContext.Reservations
            .Where(x => x.BookingStatus == BookingStatus.Booked)
            .SumAsync(x => x.TotalAmount);
    }
}
