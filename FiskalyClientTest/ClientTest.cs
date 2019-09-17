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

        public string CreateTss(string tssGuid)
        {
            Log.Information("creating tss...");
            var url = $"tss/{tssGuid}";
            var payload = $"{{\"description\": \"{tssGuid}\", \"state\": \"INITIALIZED\"}}";
            var response = client.PutAsync(url, Content(payload)).Result;
            Log.Information("response: {@Response}", response);
            return response.Content.ReadAsStringAsync().Result;
        }

        public string CreateClient(string tssGuid, string clientGuid)
        {
            var url = $"tss/{tssGuid}/client/{clientGuid}";
            var payload = $"{{\"serial_number\": \"{clientGuid}\"}}";
            return client.PutAsync(url, Content(payload)).Result.Content.ReadAsStringAsync().Result;
        }

        public string CreateTx(string tssGuid, string clientGuid, string txGuid)
        {
            var url = $"tss/{tssGuid}/tx/{txGuid}?last_revision=0";
            var payload = $"{{\"type\": \"OTHER\", \"data\": {{ \"binary\": \"test\" }}, \"state\": \"ACTIVE\", \"client_id\": \"{clientGuid}\"}}";
            return client.PutAsync(url, Content(payload)).Result.Content.ReadAsStringAsync().Result;
        }

        [SetUp]
        public void Setup()
        {
            Log.Information("logging setup...");
            Log.Logger = new LoggerConfiguration()
              .WriteTo.Console()
              .CreateLogger();

            Log.Information("client setup...");
            var apiKey = Environment.GetEnvironmentVariable("API_KEY");
            var apiSecret = Environment.GetEnvironmentVariable("API_SECRET");
            client = ClientFactory.Create(apiKey, apiSecret);
        }

        [Test]
        public void CreateTss()
        {
            var tssGuid = Guid.NewGuid().ToString();
            var response = CreateTss(tssGuid);

            var obj = JObject.Parse(response);
            var description = (string)obj["description"];
            var state = (string)obj["state"];

            Assert.AreEqual(tssGuid, description);
            Assert.AreEqual("INITIALIZED", state);
        }

        [Test]
        public void CreateClient()
        {
            var tssGuid = Guid.NewGuid().ToString();
            CreateTss(tssGuid);

            var clientGuid = Guid.NewGuid().ToString();
            var response = CreateClient(tssGuid, clientGuid);

            var obj = JObject.Parse(response);
            var serial = (string)obj["serial_number"];

            Assert.AreEqual(clientGuid, serial);
        }

        [Test]
        public void CreateTx()
        {
            var tssGuid = Guid.NewGuid().ToString();
            CreateTss(tssGuid);

            var clientGuid = Guid.NewGuid().ToString();
            CreateClient(tssGuid, clientGuid);

            var txGuid = Guid.NewGuid().ToString();
            var response = CreateTx(tssGuid, clientGuid, txGuid);

            var obj = JObject.Parse(response);
            var number = (string)obj["number"];

            Assert.AreEqual("1", number);
        }
    }
}
