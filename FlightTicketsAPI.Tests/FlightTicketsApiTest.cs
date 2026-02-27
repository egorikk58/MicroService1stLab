using FlightTicketsAPI.Models;
using FlightTicketsAPI.Tests.Factories;
using System.Net.Http.Json;

namespace FlightTicketsAPI.Tests;

public class FlightTicketsApiTest:IClassFixture<ApiFactory>
{

    private readonly HttpClient httpClient;

    public FlightTicketsApiTest(ApiFactory factory)
    {
        httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task Test_Add_100_Elements()
    {
        var dtos = Generate(100);
        var response = await httpClient.PostAsJsonAsync("api/FlightTickets/insertmany", dtos);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Test_Add_100000_Elements()
    {
        var dtos = Generate(100000);
        var response = await httpClient.PostAsJsonAsync("api/FlightTickets/insertmany", dtos);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Test_Delete_All()
    {
        var response = await httpClient.DeleteAsync("api/FlightTickets/clearall");
        response.EnsureSuccessStatusCode();
    }

    public List<DtoFlightTicket> Generate(int count)
    {
        var random = new Random();
        var airports = new[] { "SVO", "JFK", "LHR", "DXB", "HND" };
        var batch = new List<DtoFlightTicket>();
        for (int i = 0; i < count; i++)
        {
            batch.Add(new DtoFlightTicket
            {
                FlightNumber = $"{(char)random.Next(65, 91)}{(char)random.Next(65, 91)} {random.Next(100, 9999)}",
                DepartureCode = airports[random.Next(airports.Length)],
                ArrivalCode = airports[random.Next(airports.Length)],
                Seat = $"{(char)random.Next(65, 71)}{random.Next(1, 50):D2}",
                Price = random.Next(1000, 100000),
                DepartureTime = DateTime.UtcNow.AddDays(random.Next(1, 10)),
                ArrivalTime = DateTime.UtcNow.AddDays(random.Next(11, 20))
            });
        }
        return batch;
    }
}
