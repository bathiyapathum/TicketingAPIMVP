using FlightBookingApi.Models;

namespace FlightBookingApi.Repositories.Interfaces;

public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(int reservationId);
    Task<Reservation> AddAsync(Reservation reservation);
    Task UpdateAsync(Reservation reservation);
    Task<int> CountBookedAsync();
    Task<decimal> SumRevenueAsync();
}
