using Polly;
using Polly.Timeout;
using Polly.Wrap;
using Serilog;
using System;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("FiskalyClientTest")]
namespace Fiskaly.Client
{
    internal class PollyHandler : DelegatingHandler
    {
        public AsyncPolicyWrap<HttpResponseMessage> Policy;
        internal PollyHandler(HttpMessageHandler handler)
        {
            base.InnerHandler = handler;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await Policy.ExecuteAsync(
              async () =>
              {
                  Log.Information("sending request to {@Uri}...", request.RequestUri);
                  return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
              }
            ).ConfigureAwait(false);

            return response;
        }
    }
}
