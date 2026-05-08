using FlightBookingApi.DTOs.Reports;

namespace FlightBookingApi.Repositories.Interfaces;

public interface IReportRepository
{
    Task<List<RevenueReportDto>> GetRevenueAsync(DateTime? from, DateTime? to);
    Task<List<BookingsByRouteDto>> GetBookingsByRouteAsync();
    Task<SeatOccupancyDto?> GetSeatOccupancyAsync(int flightId);
    Task<List<DailyBookingTrendDto>> GetDailyBookingsAsync(DateTime? from, DateTime? to);
    Task<decimal> GetOverallOccupancyRateAsync();
}
