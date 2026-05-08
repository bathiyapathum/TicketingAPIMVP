using AutoMapper;
using FlightBookingApi.Common;
using FlightBookingApi.Data;
using FlightBookingApi.DTOs.Reservations;
using FlightBookingApi.Models;
using FlightBookingApi.Repositories.Interfaces;
using FlightBookingApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlightBookingApi.Services.Implementations;

public class ReservationService(
    FlightBookingDbContext dbContext,
    IReservationRepository reservationRepository,
    IFlightRepository flightRepository,
    ISeatRepository seatRepository,
    IMapper mapper) : IReservationService
{
    public async Task<(bool Success, string Message, ReservationDto? Data)> CreateAsync(CreateReservationRequestDto request)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        var flight = await flightRepository.GetByIdAsync(request.FlightId);
        if (flight is null)
        {
            return (false, "Flight not found.", null);
        }

        var seatStatus = await seatRepository.GetSeatStatusAsync(request.FlightId, request.SeatNumber);
        if (seatStatus is null)
        {
            return (false, "Invalid seat for selected flight.", null);
        }

        if (seatStatus.IsReserved)
        {
            return (false, "Selected seat is already reserved.", null);
        }

        var reservation = new Reservation
        {
            FlightId = request.FlightId,
            PassengerName = request.PassengerName,
            Email = request.Email,
            PassportNo = request.PassportNo,
            SeatNumber = request.SeatNumber,
            TotalAmount = flight.BasePrice,
            BookingStatus = BookingStatus.Booked,
            BookingDate = DateTime.UtcNow
        };

        await reservationRepository.AddAsync(reservation);

        seatStatus.IsReserved = true;
        seatStatus.ReservationId = reservation.ReservationId;

        reservation.Flight = flight;
        await reservationRepository.UpdateAsync(reservation);
        await transaction.CommitAsync();

        var dto = mapper.Map<ReservationDto>(reservation);
        return (true, "Reservation created successfully.", dto);
    }

    public async Task<ReservationDto?> GetByIdAsync(int reservationId)
    {
        var reservation = await reservationRepository.GetByIdAsync(reservationId);
        if (reservation is null)
        {
            return null;
        }

        return mapper.Map<ReservationDto>(reservation);
    }

    public async Task<(bool Success, string Message)> CancelAsync(int reservationId)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        var reservation = await reservationRepository.GetByIdAsync(reservationId);
        if (reservation is null)
        {
            return (false, "Reservation not found.");
        }

        if (reservation.BookingStatus == BookingStatus.Cancelled)
        {
            return (false, "Reservation already cancelled.");
        }

        var seatStatus = await seatRepository.GetSeatStatusAsync(reservation.FlightId, reservation.SeatNumber);
        if (seatStatus is not null)
        {
            seatStatus.IsReserved = false;
            seatStatus.ReservationId = null;
        }

        reservation.BookingStatus = BookingStatus.Cancelled;
        await reservationRepository.UpdateAsync(reservation);
        await transaction.CommitAsync();

        return (true, "Reservation cancelled and seat released successfully.");
    }
}
