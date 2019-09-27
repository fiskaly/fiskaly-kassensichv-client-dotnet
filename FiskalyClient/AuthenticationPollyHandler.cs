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
    internal class AuthenticationPollyHandler : DelegatingHandler
    {
        private AsyncPolicyWrap<HttpResponseMessage> _policy;
        internal AuthenticationPollyHandler(HttpMessageHandler handler)
        {
            base.InnerHandler = handler;
            SetupPolicies();
        }

        private void SetupPolicies()
        {
            Random jitterer = new Random();
            var retryPolicy = Policy
              .HandleInner<HttpRequestException>()
              .Or<TimeoutRejectedException>()
              .OrResult<HttpResponseMessage>(response => response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Unauthorized)
              .WaitAndRetryForeverAsync(
                retryAttempt => TimeSpan.FromSeconds(
                  Math.Min(Math.Pow(2, retryAttempt), Constants.MaxRetryInterval))
                  + TimeSpan.FromMilliseconds(jitterer.Next(0, 100)
                ),
                (exception, timeSpan) =>
              {
                  Log.Warning("Request failed with {@Exception}.", exception);
                  Log.Warning("Waiting {@TimeSpan} before next retry...", timeSpan);
              });

            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(
              TimeSpan.FromSeconds(Constants.HttpRequestTimeout),
              TimeoutStrategy.Pessimistic
            );

            _policy = Policy.WrapAsync(retryPolicy, timeoutPolicy);
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await _policy.ExecuteAsync(
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
