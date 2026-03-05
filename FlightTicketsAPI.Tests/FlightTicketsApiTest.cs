using FlightTicketsAPI.Models;
using FlightTicketsAPI.Tests.Factories;
using System.Net.Http.Json;
using System.Text.Json;

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
        var clearResponse = await httpClient.DeleteAsync("api/FlightTickets/clearall");
        clearResponse.EnsureSuccessStatusCode();
        const int N = 100;
        int counter = 0;
        for (int i = 0; i < N; i++)
        {
            var dto = Generate();
            var response = await httpClient.PostAsJsonAsync("api/FlightTickets", dto);
            if (response.IsSuccessStatusCode)
            {
                counter++;
            }
        }
        Assert.Equal(N, counter);
    }

    [Fact]
    public async Task Test_Add_10000_Elements()
    {
        var clearResponse = await httpClient.DeleteAsync("api/FlightTickets/clearall");
        clearResponse.EnsureSuccessStatusCode();
        const int N = 10000;
        int counter = 0;
        for (int i = 0; i < N; i++)
        {
            var dto = Generate();
            var response = await httpClient.PostAsJsonAsync("api/FlightTickets", dto);
            if (response.IsSuccessStatusCode) {
                counter++;
            }
        }
        Assert.Equal(N, counter);
    }

    [Fact]
    public async Task Test_Delete_All()
    {
        var response = await httpClient.DeleteAsync("api/FlightTickets/clearall");
        response.EnsureSuccessStatusCode();
    }

    public DtoFlightTicket Generate()
    {
        var random = new Random();
        var airports = new[] { "SVO", "JFK", "LHR", "DXB", "HND" };
        var batch = new DtoFlightTicket
        {
            FlightNumber = $"{(char)random.Next(65, 91)}{(char)random.Next(65, 91)} {random.Next(100, 9999)}",
            DepartureCode = airports[random.Next(airports.Length)],
            ArrivalCode = airports[random.Next(airports.Length)],
            Seat = $"{(char)random.Next(65, 71)}{random.Next(1, 50):D2}",
            Price = random.Next(1000, 100000),
            DepartureTime = DateTime.UtcNow.AddDays(random.Next(1, 10)),
            ArrivalTime = DateTime.UtcNow.AddDays(random.Next(11, 20))
        };
        return batch;
    }
}
