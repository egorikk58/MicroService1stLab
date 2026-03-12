using FlightTicketsAPI.Models;
using FlightTicketsAPI.Services;
using FlightTicketsAPI.Services.Impl;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add services to the container.
builder.Services.AddSingleton<IFlightTicketService, FlightTicketService>();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.Configure<FlightTicketsDBSettings>(
    builder.Configuration.GetSection("FlightTicketsDB"));

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FlightTicketsAPI V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.UseRouting();
app.UseHttpMetrics();
app.MapMetrics();

app.MapControllers();

app.Run();

public partial class Program { }