namespace FlightBookingApi.Models;

public class Flight
{
    public int FlightId { get; set; }
    public string FlightCode { get; set; } = string.Empty;
    public int RouteId { get; set; }
    public int AircraftId { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public int DurationMinutes { get; set; }
    public decimal BasePrice { get; set; }

    public Route? Route { get; set; }
    public Aircraft? Aircraft { get; set; }
    public ICollection<FlightSeatStatus> SeatStatuses { get; set; } = new List<FlightSeatStatus>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
