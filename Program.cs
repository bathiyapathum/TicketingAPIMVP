using System.Reflection;
using FlightBookingApi.Data;
using FlightBookingApi.Mapping;
using FlightBookingApi.Repositories.Implementations;
using FlightBookingApi.Repositories.Interfaces;
using FlightBookingApi.Services.Implementations;
using FlightBookingApi.Services.Interfaces;
using FlightBookingApi.Swagger;
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
    // Single document with every controller (use for Azure AI Foundry / external OpenAPI consumers).
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Flight Booking API", Version = "1.0.0" });
    options.SwaggerDoc("Flights", new OpenApiInfo { Title = "Flights APIs", Version = "1.0.0" });
    options.SwaggerDoc("Seats", new OpenApiInfo { Title = "Seats APIs", Version = "1.0.0" });
    options.SwaggerDoc("Reservations", new OpenApiInfo { Title = "Reservations APIs", Version = "1.0.0" });
    options.SwaggerDoc("Reports", new OpenApiInfo { Title = "Reports APIs", Version = "1.0.0" });

    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (string.Equals(docName, "v1", StringComparison.OrdinalIgnoreCase))
            return true;

        var groupName = apiDesc.GroupName;
        return !string.IsNullOrWhiteSpace(groupName) && string.Equals(groupName, docName, StringComparison.OrdinalIgnoreCase);
    });

    options.CustomOperationIds(apiDesc =>
    {
        static string ToSnake(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            var sb = new System.Text.StringBuilder();
            foreach (var ch in name)
            {
                if (char.IsUpper(ch) && sb.Length > 0)
                    sb.Append('_');
                sb.Append(char.ToLowerInvariant(ch));
            }

            return sb.ToString();
        }

        var controller = (apiDesc.ActionDescriptor.RouteValues.TryGetValue("controller", out var c) ? c : null) ?? "Api";
        var action = (apiDesc.ActionDescriptor.RouteValues.TryGetValue("action", out var a) ? a : null) ?? "Action";
        return $"{ToSnake(controller)}_{ToSnake(action)}";
    });

    options.DocumentFilter<CopilotCompatibleDocumentFilter>();

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
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "All APIs (OpenAPI)");
    options.SwaggerEndpoint("/swagger/Flights/swagger.json", "Flights APIs");
    options.SwaggerEndpoint("/swagger/Seats/swagger.json", "Seats APIs");
    options.SwaggerEndpoint("/swagger/Reservations/swagger.json", "Reservations APIs");
    options.SwaggerEndpoint("/swagger/Reports/swagger.json", "Reports APIs");
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
