using FlightBookingApi.Models;
using RouteEntity = FlightBookingApi.Models.Route;
using Microsoft.EntityFrameworkCore;

namespace FlightBookingApi.Data;

public class FlightBookingDbContext(DbContextOptions<FlightBookingDbContext> options) : DbContext(options)
{
    public DbSet<RouteEntity> Routes => Set<RouteEntity>();
    public DbSet<Aircraft> Aircrafts => Set<Aircraft>();
    public DbSet<Flight> Flights => Set<Flight>();
    public DbSet<Seat> Seats => Set<Seat>();
    public DbSet<FlightSeatStatus> FlightSeatStatuses => Set<FlightSeatStatus>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RouteEntity>(entity =>
        {
            entity.ToTable("Routes");
            entity.HasKey(x => x.RouteId);
            entity.Property(x => x.Origin).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Destination).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Aircraft>(entity =>
        {
            entity.ToTable("Aircrafts");
            entity.HasKey(x => x.AircraftId);
            entity.Property(x => x.AircraftName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.TotalSeats).IsRequired();
        });

        modelBuilder.Entity<Flight>(entity =>
        {
            entity.ToTable("Flights");
            entity.HasKey(x => x.FlightId);
            entity.Property(x => x.FlightCode).HasMaxLength(20).IsRequired();
            entity.Property(x => x.BasePrice).HasColumnType("decimal(18,2)");
            entity.HasOne(x => x.Route).WithMany(x => x.Flights).HasForeignKey(x => x.RouteId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Aircraft).WithMany(x => x.Flights).HasForeignKey(x => x.AircraftId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.ToTable("Seats");
            entity.HasKey(x => x.SeatId);
            entity.Property(x => x.SeatNumber).HasMaxLength(10).IsRequired();
            entity.Property(x => x.RowNumber).HasColumnName("SeatRow");
            entity.Property(x => x.ColumnLetter).HasColumnName("SeatColumn").HasMaxLength(2).IsRequired();
            entity.HasIndex(x => new { x.AircraftId, x.SeatNumber }).IsUnique();
            entity.HasOne(x => x.Aircraft).WithMany(x => x.Seats).HasForeignKey(x => x.AircraftId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FlightSeatStatus>(entity =>
        {
            entity.ToTable("FlightSeatStatus");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.SeatNumber).HasMaxLength(10).IsRequired();
            entity.HasIndex(x => new { x.FlightId, x.SeatNumber }).IsUnique();
            entity.HasOne(x => x.Flight).WithMany(x => x.SeatStatuses).HasForeignKey(x => x.FlightId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Reservation).WithMany().HasForeignKey(x => x.ReservationId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.ToTable("Reservations");
            entity.HasKey(x => x.ReservationId);
            entity.Property(x => x.PassengerName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(100).IsRequired();
            entity.Property(x => x.PassportNo).HasMaxLength(50).IsRequired();
            entity.Property(x => x.SeatNumber).HasMaxLength(10).IsRequired();
            entity.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(x => x.BookingStatus).HasConversion<string>().HasMaxLength(20);
            entity.HasOne(x => x.Flight).WithMany(x => x.Reservations).HasForeignKey(x => x.FlightId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
