using Haengma.GS;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Haengma.Tests
{
    public abstract class IntegrationTest
    {
        private static readonly IDictionary<string, string> Config = new Dictionary<string, string>
        {
            { "ConnectionStrings:DefaultConnection", "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Haengma;Integrated Security=SSPI;" }
        };

        private static readonly IConfiguration TestConfiguration = new ConfigurationBuilder()
            .AddInMemoryCollection(Config)
            .Build();

        public TestServer Server { get; }
        protected IntegrationTest()
        {
            Server = new(new WebHostBuilder().UseStartup(x => new Startup(TestConfiguration)));
        }

        public async Task<HubConnection> StartHubConnectionAsync()
        {
            var hub = NewHubConnection("game");
            await hub.StartAsync();
            return hub;
        }

        public HubConnection NewHubConnection(string hub) => new HubConnectionBuilder()
            .WithUrl(
                $"http://localhost/hub/{hub}",
                o => o.HttpMessageHandlerFactory = _ => Server.CreateHandler()
            )
            .Build();
    }
}
