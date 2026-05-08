using FlightBookingApi.DTOs.Common;
using FlightBookingApi.DTOs.Reservations;
using FlightBookingApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FlightBookingApi.Controllers;

[ApiController]
[Route("api/reservations")]
[ApiExplorerSettings(GroupName = "Reservations")]
public class ReservationsController(IReservationService reservationService) : ControllerBase
{
    /// <summary>
    /// Create a new reservation.
    /// </summary>
    /// <param name="request">Booking payload.</param>
    [HttpPost]
    [ProducesResponseType(typeof(ReservationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateReservationRequestDto request)
    {
        var result = await reservationService.CreateAsync(request);
        if (!result.Success || result.Data is null)
        {
            return BadRequest(new MessageResponseDto { Message = result.Message ?? string.Empty });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data.ReservationId }, result.Data);
    }

    /// <summary>
    /// Get reservation details by ID.
    /// </summary>
    /// <param name="id">Reservation identifier.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ReservationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var reservation = await reservationService.GetByIdAsync(id);
        return reservation is null ? NotFound() : Ok(reservation);
    }

    /// <summary>
    /// Cancel reservation and release the seat.
    /// </summary>
    /// <param name="id">Reservation identifier.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cancel(int id)
    {
        var result = await reservationService.CancelAsync(id);
        var body = new MessageResponseDto { Message = result.Message ?? string.Empty };
        return result.Success ? Ok(body) : BadRequest(body);
    }
}
