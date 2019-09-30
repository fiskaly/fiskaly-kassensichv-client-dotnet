using Serilog;
using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("FiskalyClientTest")]
namespace Fiskaly.Client
{
    public static class ClientFactory
    {
        public static async ValueTask<HttpClient> Create(string apiKey, string apiSecret)
        {
            Log.Information("creating fiskaly client...");
            var authenticationHandler = new AuthenticationHandler(new HttpClientHandler(), apiKey, apiSecret);
            await authenticationHandler.Start().ConfigureAwait(false);
            var pollyHandler = new PollyHandler(authenticationHandler) {
              Policy = PollyPolicyFactory.CreateGeneralPolicy()
            };
            var transactionHandler = new TransactionHandler(pollyHandler);
            var requestUriEnforcementHandler = new RequestUriEnforcementHandler(transactionHandler);
            HttpClient client = new HttpClient(requestUriEnforcementHandler)
            {
                BaseAddress = new Uri(Constants.BaseAddress),
                Timeout = Timeout.InfiniteTimeSpan
            };
            return client;
        }
    }
}
