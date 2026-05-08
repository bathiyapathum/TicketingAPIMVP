using FlightBookingApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FlightBookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "Reports")]
public class ReportsController(IReportService reportService) : ControllerBase
{
    /// <summary>
    /// Get total revenue grouped by route and flight.
    /// </summary>
    [HttpGet("revenue")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Revenue([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var data = await reportService.GetRevenueAsync(from, to);
        return Ok(data);
    }

    /// <summary>
    /// Get booking count per route.
    /// </summary>
    [HttpGet("bookings-by-route")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> BookingsByRoute()
    {
        var data = await reportService.GetBookingsByRouteAsync();
        return Ok(data);
    }

    /// <summary>
    /// Get seat occupancy percentage for a flight.
    /// </summary>
    [HttpGet("seat-occupancy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SeatOccupancy([FromQuery] int flightId)
    {
        var data = await reportService.GetSeatOccupancyAsync(flightId);
        return data is null ? NotFound() : Ok(data);
    }

    /// <summary>
    /// Get daily booking trend.
    /// </summary>
    [HttpGet("daily-bookings")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DailyBookings([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var data = await reportService.GetDailyBookingsAsync(from, to);
        return Ok(data);
    }

    /// <summary>
    /// Get high-level summary for AI agents.
    /// </summary>
    [HttpGet("agent-summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AgentSummary()
    {
        var data = await reportService.GetAgentSummaryAsync();
        return Ok(data);
    }
}
