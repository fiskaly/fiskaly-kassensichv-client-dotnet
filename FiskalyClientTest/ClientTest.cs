using System.Threading.Tasks;
using NUnit.Framework;
using System.Net.Http;
using System.Text;
using Fiskaly.Client;
using Serilog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Fiskaly.Client.Test
{
    public class ClientTest
    {
        private HttpClient client;

        public StringContent Content(string payload)
        {
            return new StringContent(payload, Encoding.UTF8, "application/json");
        }

        public async Task<String> CreateTss(string tssGuid)
        {
            Log.Information("creating tss...");
            var url = $"tss/{tssGuid}";
            var payload = $"{{\"description\": \"{tssGuid}\", \"state\": \"INITIALIZED\"}}";
            var response = await client.PutAsync(url, Content(payload)).ConfigureAwait(false);
            Log.Information("response: {@Response}", response);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public async Task<String> CreateClient(string tssGuid, string clientGuid)
        {
            var url = $"tss/{tssGuid}/client/{clientGuid}";
            var payload = $"{{\"serial_number\": \"{clientGuid}\"}}";
            var response = await client.PutAsync(url, Content(payload)).ConfigureAwait(false);
            Log.Information("response: {@Response}", response);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public async Task<String> CreateTx(string tssGuid, string clientGuid, string txGuid)
        {
            var url = $"tss/{tssGuid}/tx/{txGuid}?last_revision=0";
            var payload = $"{{\"type\": \"OTHER\", \"data\": {{ \"binary\": \"test\" }}, \"state\": \"ACTIVE\", \"client_id\": \"{clientGuid}\"}}";
            var response = await client.PutAsync(url, Content(payload)).ConfigureAwait(false);
            Log.Information("response: {@Response}", response);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

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
            var response = await CreateTss(tssGuid).ConfigureAwait(false);

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
            await CreateTss(tssGuid).ConfigureAwait(false);

            var clientGuid = Guid.NewGuid().ToString();
            var response = await CreateClient(tssGuid, clientGuid).ConfigureAwait(false);

            var obj = JObject.Parse(response);
            var serial = (string)obj["serial_number"];

            Assert.AreEqual(clientGuid, serial);
        }

        [Test]
        public async Task CreateTx()
        {
            var tssGuid = Guid.NewGuid().ToString();
            await CreateTss(tssGuid).ConfigureAwait(false);

            var clientGuid = Guid.NewGuid().ToString();
            await CreateClient(tssGuid, clientGuid).ConfigureAwait(false);

            var txGuid = Guid.NewGuid().ToString();
            var response = await CreateTx(tssGuid, clientGuid, txGuid).ConfigureAwait(false);

            var obj = JObject.Parse(response);
            var number = (string)obj["number"];

            Assert.AreEqual("1", number);
        }
    }
}
