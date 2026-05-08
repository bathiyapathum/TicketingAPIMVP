using AutoMapper;
using FlightBookingApi.DTOs.Flights;
using FlightBookingApi.DTOs.Reservations;
using FlightBookingApi.Models;

namespace FlightBookingApi.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Flight, FlightListItemDto>()
            .ForMember(d => d.Origin, o => o.MapFrom(s => s.Route!.Origin))
            .ForMember(d => d.Destination, o => o.MapFrom(s => s.Route!.Destination));

        CreateMap<Reservation, ReservationDto>()
            .ForMember(d => d.FlightCode, o => o.MapFrom(s => s.Flight!.FlightCode))
            .ForMember(d => d.BookingStatus, o => o.MapFrom(s => s.BookingStatus.ToString()));
    }
}
