using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Runtime.CompilerServices;
using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("FiskalyClientTest")]
namespace Fiskaly.Client
{
    internal class TransactionHandler : DelegatingHandler
    {
        internal TransactionHandler(HttpMessageHandler handler)
        {
            base.InnerHandler = handler;
        }

        private async Task InterceptTxRequest(HttpRequestMessage request)
        {
            Log.Information("intercepting tx request...");
            var body = await request.Content.ReadAsStringAsync();
            Log.Information("body of tx request: {Body}", body);
            var smaResponse = Sma.SignTx(body);
            Log.Information("SMA reponse: {SmaReponse}", smaResponse);
            var obj = JObject.Parse(smaResponse);
            var result = obj["result"].ToString();
            request.Content = new StringContent(result, Encoding.UTF8, "application/json");
            Log.Information("new request content: {@Content}", request.Content);
            request.RequestUri = new Uri(OverwriteTxUrl(request.RequestUri.OriginalString));
            Log.Information("new request uri: {@Uri}", request.RequestUri);
        }

        private string OverwriteTxUrl(string url)
        {
            string[] arr = url.Split('?');

            if (arr.Length == 1)
            {
                return $"{url}/log";
            }
            else
            {
                return $"{arr[0]}/log?{arr[1]}";
            }
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Put && Regex.IsMatch(request.RequestUri.OriginalString, "\\/tss\\/.+\\/tx\\/.+"))
            {
                await InterceptTxRequest(request);
            }

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return response;
        }
    }
}
