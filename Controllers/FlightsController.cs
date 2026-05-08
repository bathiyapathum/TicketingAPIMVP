using FlightBookingApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FlightBookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "Flights")]
public class FlightsController(IFlightService flightService) : ControllerBase
{
    /// <summary>
    /// Get all flights with optional route/date filtering.
    /// </summary>
    /// <param name="origin">Origin city.</param>
    /// <param name="destination">Destination city.</param>
    /// <param name="date">Departure date (yyyy-MM-dd).</param>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? origin, [FromQuery] string? destination, [FromQuery] DateTime? date)
    {
        var flights = await flightService.GetFlightsAsync(origin, destination, date);
        return Ok(flights);
    }

    /// <summary>
    /// Get flight details by ID.
    /// </summary>
    /// <param name="id">Flight identifier.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var flight = await flightService.GetByIdAsync(id);
        return flight is null ? NotFound() : Ok(flight);
    }

    /// <summary>
    /// Search flights for AI agent usage.
    /// </summary>
    /// <param name="origin">Origin city.</param>
    /// <param name="destination">Destination city.</param>
    /// <param name="date">Departure date (yyyy-MM-dd).</param>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string? origin, [FromQuery] string? destination, [FromQuery] DateTime? date)
    {
        var flights = await flightService.GetFlightsAsync(origin, destination, date);
        return Ok(flights);
    }
}
