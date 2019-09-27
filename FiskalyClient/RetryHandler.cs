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
    internal class RetryHandler : DelegatingHandler
    {
        private AsyncRetryPolicy _policy;
        internal RetryHandler(HttpMessageHandler handler)
        {
            base.InnerHandler = handler;
            SetupRetryWithExponentialBackoff();
        }

        private void SetupRetryWithExponentialBackoff()
        {
            Random jitterer = new Random();
            _policy = Policy
              .HandleInner<HttpRequestException>()
              .WaitAndRetryForeverAsync(
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(jitterer.Next(0, 100)),
                (exception, timeSpan) =>
              {
                  Log.Warning("Request failed with {@Exception}.", exception);
                  Log.Warning("Waiting {@TimeSpan} before next retry...", timeSpan);
              });
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await _policy.ExecuteAsync(
              async () =>
              {
                  Log.Information("sending request to {@Uri}...", request.RequestUri);
                  var innerResponse = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                  innerResponse.EnsureSuccessStatusCode();
                  return innerResponse;
              }
            ).ConfigureAwait(false);

            return response;
        }
    }
}
