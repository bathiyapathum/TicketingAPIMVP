using FlightBookingApi.DTOs.Seats;
using FlightBookingApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FlightBookingApi.Controllers;

[ApiController]
[Route("api/flights/{flightId:int}/seats")]
[ApiExplorerSettings(GroupName = "Seats")]
public class SeatsController(ISeatService seatService) : ControllerBase
{
    /// <summary>
    /// Get seat matrix for the selected flight.
    /// </summary>
    /// <param name="flightId">Flight identifier.</param>
    [HttpGet]
    [ProducesResponseType(typeof(List<SeatStatusDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByFlight(int flightId)
    {
        var seats = await seatService.GetSeatsByFlightAsync(flightId);
        return Ok(seats);
    }
}
