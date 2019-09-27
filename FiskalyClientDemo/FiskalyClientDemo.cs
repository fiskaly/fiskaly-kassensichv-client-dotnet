using System;
using System.Net.Http;
using Fiskaly.Client;
using Serilog;
using System.Text;
using System.Threading.Tasks;

namespace Fiskaly.Client
{
    class Demo
    {
        static String ApiKey = Environment.GetEnvironmentVariable("API_KEY");
        static String ApiSecret = Environment.GetEnvironmentVariable("API_SECRET");
        static HttpClient client;

        public static StringContent Content(string payload)
        {
            return new StringContent(payload, Encoding.UTF8, "application/json");
        }

        public static async Task<String> CreateTss()
        {
            Log.Information("creating tss...");
            var tssGuid = Guid.NewGuid().ToString();
            var url = $"tss/{tssGuid}";
            var payload = $"{{\"description\": \"{tssGuid}\", \"state\": \"INITIALIZED\"}}";

            HttpResponseMessage response = await client
              .PutAsync(url, Content(payload))
              .ConfigureAwait(false);

            String content = await response.Content
              .ReadAsStringAsync()
              .ConfigureAwait(false);

            return content;
        }

        public static async Task<String> ListTss()
        {
            Log.Information("listing tss...");

            HttpResponseMessage response = await client
              .GetAsync("tss")
              .ConfigureAwait(false);

            String content = await response.Content
              .ReadAsStringAsync()
              .ConfigureAwait(false);

            return content;
        }

        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
              .WriteTo.Console()
              .CreateLogger();

            client = await ClientFactory.Create(ApiKey, ApiSecret).ConfigureAwait(false);
            var createTssResponse = await CreateTss().ConfigureAwait(false);
            var listTssResponse = await ListTss().ConfigureAwait(false);
            Log.Information("createTssResponse: {Response}", createTssResponse);
            Log.Information("listTssResponse: {Response}", listTssResponse);
        }
    }
}
