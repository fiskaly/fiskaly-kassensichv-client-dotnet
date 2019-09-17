using System;
using System.Net.Http;
using Fiskaly.Client;
using Serilog;
using System.Text;

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

        public static string CreateTss()
        {
            Log.Information("creating tss...");
            var tssGuid = Guid.NewGuid().ToString();
            var url = $"tss/{tssGuid}";
            var payload = $"{{\"description\": \"{tssGuid}\", \"state\": \"INITIALIZED\"}}";
            return client
              .PutAsync(url, Content(payload))
              .Result
              .Content
              .ReadAsStringAsync()
              .Result;
        }

        public static string ListTss()
        {
            Log.Information("listing tss...");
            return client
              .GetAsync("tss")
              .Result
              .Content
              .ReadAsStringAsync()
              .Result;
        }

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
              .WriteTo.Console()
              .CreateLogger();

            client = ClientFactory.Create(ApiKey, ApiSecret);
            var createTssResponse = CreateTss();
            var listTssResponse = ListTss();
            Log.Information("createTssResponse: {Response}", createTssResponse);
            Log.Information("listTssResponse: {Response}", listTssResponse);
        }
    }
}
