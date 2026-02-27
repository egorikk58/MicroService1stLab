using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mongo2Go;
using FlightTicketsAPI.Models;

namespace FlightTicketsAPI.Tests.Factories
{
    public class ApiFactory : WebApplicationFactory<Program>
    {
        public MongoDbRunner dbRunner { get; }

        public ApiFactory()
        {
            dbRunner = MongoDbRunner.Start();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.Configure<FlightTicketsDBSettings>(options =>
                {
                    options.ConnectionString = dbRunner.ConnectionString;
                    options.DatabaseName = "TestDatabase";
                    options.CollectionName = "FlightTicketsBase";
                });
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            dbRunner.Dispose();
        }
    }
}
