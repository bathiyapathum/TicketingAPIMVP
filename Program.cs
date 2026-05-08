using System.Reflection;
using FlightBookingApi.Data;
using FlightBookingApi.Mapping;
using FlightBookingApi.Repositories.Implementations;
using FlightBookingApi.Repositories.Interfaces;
using FlightBookingApi.Services.Implementations;
using FlightBookingApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<FlightBookingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<IFlightRepository, FlightRepository>();
builder.Services.AddScoped<ISeatRepository, SeatRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();

builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddScoped<ISeatService, SeatService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IReportService, ReportService>();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("Flights", new OpenApiInfo { Title = "Flights APIs", Version = "v1" });
    options.SwaggerDoc("Seats", new OpenApiInfo { Title = "Seats APIs", Version = "v1" });
    options.SwaggerDoc("Reservations", new OpenApiInfo { Title = "Reservations APIs", Version = "v1" });
    options.SwaggerDoc("Reports", new OpenApiInfo { Title = "Reports APIs", Version = "v1" });

    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        var groupName = apiDesc.GroupName;
        return !string.IsNullOrWhiteSpace(groupName) && string.Equals(groupName, docName, StringComparison.OrdinalIgnoreCase);
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

var app = builder.Build();

var applyMigrationsOnStartup = builder.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FlightBookingDbContext>();
    await DbInitializer.SeedAsync(dbContext, applyMigrationsOnStartup);
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/Flights/swagger.json", "Flights APIs");
    options.SwaggerEndpoint("/swagger/Seats/swagger.json", "Seats APIs");
    options.SwaggerEndpoint("/swagger/Reservations/swagger.json", "Reservations APIs");
    options.SwaggerEndpoint("/swagger/Reports/swagger.json", "Reports APIs");
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
