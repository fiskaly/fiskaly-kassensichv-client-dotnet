using Polly;
using Polly.Retry;
using Serilog;
using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("FiskalyClientTest")]
namespace Fiskaly.Client
{
    internal class RequestUriEnforcementHandler : DelegatingHandler
    {
        internal RequestUriEnforcementHandler(HttpMessageHandler handler)
        {
            base.InnerHandler = handler;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.RequestUri.AbsoluteUri.StartsWith(Constants.BaseAddress))
            {
                throw new InvalidRequestUriException($"The fiskaly client may only be used for requests to {Constants.BaseAddress}");
            }

            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return response;
        }
    }
}
