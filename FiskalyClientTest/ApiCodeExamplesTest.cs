using System.Threading.Tasks;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Text;
using Serilog;
using Newtonsoft.Json.Linq;
using System;

namespace Fiskaly.Client.Test
{
    public class ApiCodeExamplesTest
    {
        private static string tssId;
        private static string clientId;
        private static string serialNumber;
        private static string exportId;
        private static string txId;
        private static int lastRevision;

        [OneTimeSetUp]
        public async Task Setup()
        {
            Log.Logger = new LoggerConfiguration()
              .WriteTo.Console()
              .CreateLogger();

            var apiKey = Environment.GetEnvironmentVariable("API_KEY");
            var apiSecret = Environment.GetEnvironmentVariable("API_SECRET");
            HttpClient client = await ClientFactory.Create(apiKey, apiSecret).ConfigureAwait(false);

            tssId = Guid.NewGuid().ToString();
            await Helper.CreateTss(client, tssId).ConfigureAwait(false);
            clientId = Guid.NewGuid().ToString();
            serialNumber = Guid.NewGuid().ToString();
            await Helper.CreateClient(client, tssId, clientId).ConfigureAwait(false);
            exportId = Guid.NewGuid().ToString();
            await Helper.TriggerExport(client, tssId, exportId).ConfigureAwait(false);
            txId = Guid.NewGuid().ToString();
            await Helper.CreateTx(client, tssId, clientId, txId).ConfigureAwait(false);
            lastRevision = 1;
        }

        [Test]
        public async Task ListAllClients()
        {
            String ApiKey = Environment.GetEnvironmentVariable("API_KEY");
            String ApiSecret = Environment.GetEnvironmentVariable("API_SECRET");
            HttpClient client = await ClientFactory.Create(ApiKey, ApiSecret).ConfigureAwait(false);

            var url = "client";
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
            String content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var obj = JObject.Parse(content);

            // The code example ends here.

            Console.WriteLine(obj);
            Assert.AreEqual("CLIENT_LIST", (string)obj["_type"]);
        }

        [Test]
        public async Task ListClients()
        {
            String ApiKey = Environment.GetEnvironmentVariable("API_KEY");
            String ApiSecret = Environment.GetEnvironmentVariable("API_SECRET");
            HttpClient client = await ClientFactory.Create(ApiKey, ApiSecret).ConfigureAwait(false);

            // var tssId = "...";
            var url = $"tss/{tssId}/client";
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
            String content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var obj = JObject.Parse(content);

            // The code example ends here.

            Console.WriteLine(obj);
            Assert.AreEqual("CLIENT_LIST", (string)obj["_type"]);
        }

        [Test]
        public async Task RetrieveClient()
        {
            String ApiKey = Environment.GetEnvironmentVariable("API_KEY");
            String ApiSecret = Environment.GetEnvironmentVariable("API_SECRET");
            HttpClient client = await ClientFactory.Create(ApiKey, ApiSecret).ConfigureAwait(false);

            // var tssId = "...";
            // var clientId = "...";
            var url = $"tss/{tssId}/client/{clientId}";
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
            String content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var obj = JObject.Parse(content);

            // The code example ends here.

            Console.WriteLine(obj);
            Assert.AreEqual("CLIENT", (string)obj["_type"]);
            Assert.AreEqual(clientId, (string)obj["_id"]);
        }

        [Test]
        public async Task UpsertClient()
        {
            String ApiKey = Environment.GetEnvironmentVariable("API_KEY");
            String ApiSecret = Environment.GetEnvironmentVariable("API_SECRET");
            HttpClient client = await ClientFactory.Create(ApiKey, ApiSecret).ConfigureAwait(false);

            // var tssId = "...";
            // var clientId = "...";
            // var serialNumber = "...";
            var url = $"tss/{tssId}/client/{clientId}";
            var data = $"{{\"serial_number\": \"{serialNumber}\"}}";
            var payload = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(url, payload).ConfigureAwait(false);
            String content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var obj = JObject.Parse(content);

            // The code example ends here.

            Console.WriteLine(obj);
            Assert.AreEqual("CLIENT", (string)obj["_type"]);
            Assert.AreEqual(clientId, (string)obj["_id"]);
        }

        [Test]
        public async Task CancelExport()
        {
            String ApiKey = Environment.GetEnvironmentVariable("API_KEY");
            String ApiSecret = Environment.GetEnvironmentVariable("API_SECRET");
            HttpClient client = await ClientFactory.Create(ApiKey, ApiSecret).ConfigureAwait(false);

            // var tssId = "...";
            // var exportId = "...";
            var url = $"tss/{tssId}/export/{exportId}";
            HttpResponseMessage response = await client.DeleteAsync(url).ConfigureAwait(false);
            String content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var obj = JObject.Parse(content);

            // The code example ends here.

            Console.WriteLine(obj);
            Assert.AreEqual("EXPORT", (string)obj["_type"]);
            Assert.AreEqual(exportId, (string)obj["_id"]);
        }

        [Test]
        public async Task ListAllExports()
        {
            String ApiKey = Environment.GetEnvironmentVariable("API_KEY");
            String ApiSecret = Environment.GetEnvironmentVariable("API_SECRET");
            HttpClient client = await ClientFactory.Create(ApiKey, ApiSecret).ConfigureAwait(false);

            var url = "export";
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
            String content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var obj = JObject.Parse(content);

            // The code example ends here.

            Console.WriteLine(obj);
            Assert.AreEqual("EXPORT_LIST", (string)obj["_type"]);
        }

        [Test]
        public async Task ListExports()
        {
            String ApiKey = Environment.GetEnvironmentVariable("API_KEY");
            String ApiSecret = Environment.GetEnvironmentVariable("API_SECRET");
            HttpClient client = await ClientFactory.Create(ApiKey, ApiSecret).ConfigureAwait(false);

            // var tssId = "...";
            var url = $"tss/{tssId}/export";
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
            String content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var obj = JObject.Parse(content);

            // The code example ends here.

            Console.WriteLine(obj);
            Assert.AreEqual("EXPORT_LIST", (string)obj["_type"]);
        }

        [Test]
        public async Task RetrieveExport()
        {
            String ApiKey = Environment.GetEnvironmentVariable("API_KEY");
            String ApiSecret = Environment.GetEnvironmentVariable("API_SECRET");
            HttpClient client = await ClientFactory.Create(ApiKey, ApiSecret).ConfigureAwait(false);

            // var tssId = "...";
            // var exportId = "...";
            var url = $"tss/{tssId}/export/{exportId}";
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
            String content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var obj = JObject.Parse(content);

            // The code example ends here.

            Console.WriteLine(obj);
            Assert.AreEqual("EXPORT", (string)obj["_type"]);
            Assert.AreEqual(exportId, (string)obj["_id"]);
        }

        [Test]
        public async Task TriggerExport()
        {
            String ApiKey = Environment.GetEnvironmentVariable("API_KEY");
            String ApiSecret = Environment.GetEnvironmentVariable("API_SECRET");
            HttpClient client = await ClientFactory.Create(ApiKey, ApiSecret).ConfigureAwait(false);

            // var tssId = "...";
            var exportId = Guid.NewGuid().ToString();
            var url = $"tss/{tssId}/export/{exportId}";
            var data = "{}";
            var payload = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(url, payload).ConfigureAwait(false);
            String content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var obj = JObject.Parse(content);

            // The code example ends here.

            Console.WriteLine(obj);
            Assert.AreEqual("EXPORT", (string)obj["_type"]);
            Assert.AreEqual(exportId, (string)obj["_id"]);
        }

        [Test]
        public async Task ListTss()
        {
            String ApiKey = Environment.GetEnvironmentVariable("API_KEY");
            String ApiSecret = Environment.GetEnvironmentVariable("API_SECRET");
            HttpClient client = await ClientFactory.Create(ApiKey, ApiSecret).ConfigureAwait(false);

            var url = "tss";
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
            String content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var obj = JObject.Parse(content);

            // The code example ends here.

            Console.WriteLine(obj);
            Assert.AreEqual("TSS_LIST", (string)obj["_type"]);
        }

        [Test]
        public async Task RetrieveTss()
        {
            String ApiKey = Environment.GetEnvironmentVariable("API_KEY");
            String ApiSecret = Environment.GetEnvironmentVariable("API_SECRET");
            HttpClient client = await ClientFactory.Create(ApiKey, ApiSecret).ConfigureAwait(false);

            // var tssId = "...";
            var url = $"tss/{tssId}";
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
            String content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var obj = JObject.Parse(content);

            // The code example ends here.

            Console.WriteLine(obj);
            Assert.AreEqual("TSS", (string)obj["_type"]);
            Assert.AreEqual(tssId, (string)obj["_id"]);
        }

        [Test]
        public async Task UpsertTss()
        {
            String ApiKey = Environment.GetEnvironmentVariable("API_KEY");
            String ApiSecret = Environment.GetEnvironmentVariable("API_SECRET");
            HttpClient client = await ClientFactory.Create(ApiKey, ApiSecret).ConfigureAwait(false);

            // var tssId = "...";
            var url = $"tss/{tssId}";
            var data = $"{{\"state\": \"INITIALIZED\"}}";
            var payload = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(url, payload).ConfigureAwait(false);
            String content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var obj = JObject.Parse(content);

            // The code example ends here.

            Console.WriteLine(obj);
            Assert.AreEqual("TSS", (string)obj["_type"]);
            Assert.AreEqual(tssId, (string)obj["_id"]);
        }

        [Test]
        public async Task ListAllTransactions()
        {
            String ApiKey = Environment.GetEnvironmentVariable("API_KEY");
            String ApiSecret = Environment.GetEnvironmentVariable("API_SECRET");
            HttpClient client = await ClientFactory.Create(ApiKey, ApiSecret).ConfigureAwait(false);

            var url = "tx";
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
            String content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var obj = JObject.Parse(content);

            // The code example ends here.

            Console.WriteLine(obj);
            Assert.AreEqual("TRANSACTION_LIST", (string)obj["_type"]);
        }

        [Test]
        public async Task ListTransactions()
        {
            String ApiKey = Environment.GetEnvironmentVariable("API_KEY");
            String ApiSecret = Environment.GetEnvironmentVariable("API_SECRET");
            HttpClient client = await ClientFactory.Create(ApiKey, ApiSecret).ConfigureAwait(false);

            // var tssId = "...";
            var url = $"tss/{tssId}/tx";
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
            String content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var obj = JObject.Parse(content);

            // The code example ends here.

            Console.WriteLine(obj);
            Assert.AreEqual("TRANSACTION_LIST", (string)obj["_type"]);
        }

        [Test]
        public async Task RetrieveTransaction()
        {
            String ApiKey = Environment.GetEnvironmentVariable("API_KEY");
            String ApiSecret = Environment.GetEnvironmentVariable("API_SECRET");
            HttpClient client = await ClientFactory.Create(ApiKey, ApiSecret).ConfigureAwait(false);

            // var tssId = "...";
            // var txId = "...";
            var url = $"tss/{tssId}/tx/{txId}";
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
            String content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var obj = JObject.Parse(content);

            // The code example ends here.

            Console.WriteLine(obj);
            Assert.AreEqual("TRANSACTION", (string)obj["_type"]);
            Assert.AreEqual(txId, (string)obj["_id"]);
        }

        [Test]
        public async Task RetrieveTransactionLog()
        {
            String ApiKey = Environment.GetEnvironmentVariable("API_KEY");
            String ApiSecret = Environment.GetEnvironmentVariable("API_SECRET");
            HttpClient client = await ClientFactory.Create(ApiKey, ApiSecret).ConfigureAwait(false);

            // var tssId = "...";
            // var txId = "...";
            var url = $"tss/{tssId}/tx/{txId}/log";
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
            var content = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

            // The code example ends here.

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task UpsertTransaction()
        {
            String ApiKey = Environment.GetEnvironmentVariable("API_KEY");
            String ApiSecret = Environment.GetEnvironmentVariable("API_SECRET");
            HttpClient client = await ClientFactory.Create(ApiKey, ApiSecret).ConfigureAwait(false);

            // var tssId = "...";
            // var txId = "...";
            // var clientId = "...";
            // var lastRevision = "...";
            var url = $"tss/{tssId}/tx/{txId}?last_revision={lastRevision}";
            var data = $"{{\"type\": \"OTHER\", \"data\": {{ \"aeao\": {{ \"other\": {{}} }} }}, \"state\": \"ACTIVE\", \"client_id\": \"{clientId}\"}}";
            var payload = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(url, payload).ConfigureAwait(false);
            String content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var obj = JObject.Parse(content);

            // The code example ends here.

            Console.WriteLine(obj);
            Assert.AreEqual("TRANSACTION", (string)obj["_type"]);
            Assert.AreEqual(txId, (string)obj["_id"]);
        }
    }
}
