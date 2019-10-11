using System.Threading.Tasks;
using NUnit.Framework;
using System.Net.Http;
using Serilog;
using Newtonsoft.Json.Linq;
using System;

namespace Fiskaly.Client.Test
{
    public class ClientTest
    {
        private HttpClient client;

        [SetUp]
        public async Task Setup()
        {
            Log.Information("logging setup...");
            Log.Logger = new LoggerConfiguration()
              .WriteTo.Console()
              .CreateLogger();

            Log.Information("client setup...");
            var apiKey = Environment.GetEnvironmentVariable("API_KEY");
            var apiSecret = Environment.GetEnvironmentVariable("API_SECRET");
            client = await ClientFactory.Create(apiKey, apiSecret).ConfigureAwait(false);
        }

        [Test]
        public async Task CreateTss()
        {
            var tssGuid = Guid.NewGuid().ToString();
            var response = await Helper.CreateTss(client, tssGuid).ConfigureAwait(false);

            var obj = JObject.Parse(response);
            var description = (string)obj["description"];
            var state = (string)obj["state"];

            Assert.AreEqual(tssGuid, description);
            Assert.AreEqual("INITIALIZED", state);
        }

        [Test]
        public async Task CreateClient()
        {
            var tssGuid = Guid.NewGuid().ToString();
            await Helper.CreateTss(client, tssGuid).ConfigureAwait(false);

            var clientGuid = Guid.NewGuid().ToString();
            var response = await Helper.CreateClient(client, tssGuid, clientGuid).ConfigureAwait(false);

            var obj = JObject.Parse(response);
            var serial = (string)obj["serial_number"];

            Assert.AreEqual(clientGuid, serial);
        }

        [Test]
        public async Task CreateTx()
        {
            var tssGuid = Guid.NewGuid().ToString();
            await Helper.CreateTss(client, tssGuid).ConfigureAwait(false);

            var clientGuid = Guid.NewGuid().ToString();
            await Helper.CreateClient(client, tssGuid, clientGuid).ConfigureAwait(false);

            var txGuid = Guid.NewGuid().ToString();
            var response = await Helper.CreateTx(client, tssGuid, clientGuid, txGuid).ConfigureAwait(false);

            var obj = JObject.Parse(response);
            var number = (string)obj["number"];

            Assert.AreEqual("1", number);
        }
    }
}
