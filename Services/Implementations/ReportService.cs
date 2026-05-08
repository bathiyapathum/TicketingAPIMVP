using FlightBookingApi.DTOs.Reports;
using FlightBookingApi.Repositories.Interfaces;
using FlightBookingApi.Services.Interfaces;

namespace FlightBookingApi.Services.Implementations;

public class ReportService(
    IReportRepository reportRepository,
    IFlightRepository flightRepository,
    IReservationRepository reservationRepository) : IReportService
{
    public Task<List<RevenueReportDto>> GetRevenueAsync(DateTime? from, DateTime? to)
        => reportRepository.GetRevenueAsync(from, to);

    public Task<List<BookingsByRouteDto>> GetBookingsByRouteAsync()
        => reportRepository.GetBookingsByRouteAsync();

    public Task<SeatOccupancyDto?> GetSeatOccupancyAsync(int flightId)
        => reportRepository.GetSeatOccupancyAsync(flightId);

    public Task<List<DailyBookingTrendDto>> GetDailyBookingsAsync(DateTime? from, DateTime? to)
        => reportRepository.GetDailyBookingsAsync(from, to);

    public async Task<AgentSummaryDto> GetAgentSummaryAsync()
    {
        return new AgentSummaryDto
        {
            TotalFlights = await flightRepository.CountAsync(),
            TotalBookings = await reservationRepository.CountBookedAsync(),
            TotalRevenue = await reservationRepository.SumRevenueAsync(),
            OccupancyRate = await reportRepository.GetOverallOccupancyRateAsync()
        };
    }
}
