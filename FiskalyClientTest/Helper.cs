using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Serilog;
using System;

namespace Fiskaly.Client.Test
{
    public static class Helper
    {
        public static StringContent Content(string payload)
        {
            return new StringContent(payload, Encoding.UTF8, "application/json");
        }

        public static async Task<String> CreateTss(HttpClient client, string tssGuid)
        {
            Log.Information("creating tss...");
            var url = $"tss/{tssGuid}";
            var payload = $"{{\"description\": \"{tssGuid}\", \"state\": \"INITIALIZED\"}}";
            var response = await client.PutAsync(url, Content(payload)).ConfigureAwait(false);
            Log.Information("response: {@Response}", response);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public static async Task<String> CreateClient(HttpClient client, string tssGuid, string clientGuid)
        {
            var url = $"tss/{tssGuid}/client/{clientGuid}";
            var payload = $"{{\"serial_number\": \"{clientGuid}\"}}";
            var response = await client.PutAsync(url, Content(payload)).ConfigureAwait(false);
            Log.Information("response: {@Response}", response);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public static async Task<String> CreateTx(HttpClient client, string tssGuid, string clientGuid, string txGuid)
        {
            var url = $"tss/{tssGuid}/tx/{txGuid}?last_revision=0";
            var payload = $"{{\"type\": \"OTHER\", \"data\": {{ \"binary\": \"test\" }}, \"state\": \"ACTIVE\", \"client_id\": \"{clientGuid}\"}}";
            var response = await client.PutAsync(url, Content(payload)).ConfigureAwait(false);
            Log.Information("response: {@Response}", response);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public static async Task<String> TriggerExport(HttpClient client, string tssGuid, string exportGuid)
        {
            Log.Information("triggering export...");
            var url = $"tss/{tssGuid}/export/{exportGuid}";
            var payload = "{}";
            var response = await client.PutAsync(url, Content(payload)).ConfigureAwait(false);
            Log.Information("response: {@Response}", response);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}
