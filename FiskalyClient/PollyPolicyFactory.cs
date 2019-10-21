using Polly;
using Polly.Timeout;
using Polly.Wrap;
using Serilog;
using System;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FiskalyClientTest")]
namespace Fiskaly.Client
{
    public static class PollyPolicyFactory
    {
        private static AsyncPolicyWrap<HttpResponseMessage> CreatePolicy(Func<HttpResponseMessage, Boolean> httpResponseMessageFunc)
        {
            Random jitterer = new Random();
            var retryPolicy = Policy
              .HandleInner<HttpRequestException>()
              .Or<TimeoutRejectedException>()
              .OrResult<HttpResponseMessage>(httpResponseMessageFunc)
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

            var policy = Policy.WrapAsync(retryPolicy, timeoutPolicy);

            return policy;
        }
        public static AsyncPolicyWrap<HttpResponseMessage> CreateGeneralPolicy()
        {
            return CreatePolicy(response => response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.BadRequest);
        }
        public static AsyncPolicyWrap<HttpResponseMessage> CreateAuthPolicy()
        {
            return CreatePolicy(response => response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Unauthorized);
        }
    }
}
