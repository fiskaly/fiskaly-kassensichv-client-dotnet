using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("FiskalyClientTest")]
namespace Fiskaly.Client
{
    internal class AuthenticationHandler : DelegatingHandler
    {
        private string _apiKey;
        private string _apiSecret;
        private AuthenticationContext _authenticationContext;
        private HttpClient _authenticationClient;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        internal AuthenticationHandler(HttpMessageHandler handler, string apiKey, string apiSecret)
        {
            Log.Information("initializing authentication handler...");
            base.InnerHandler = handler;
            _apiKey = apiKey;
            _apiSecret = apiSecret;

            var httpClientHandler = new HttpClientHandler();
            var authenticationRetryHandler = new AuthenticationRetryHandler(httpClientHandler);
            _authenticationClient = new HttpClient(authenticationRetryHandler)
            {
                BaseAddress = new Uri(Constants.BaseAddress)
            };

            _authenticationContext = new AuthenticationContext();
        }

        public async Task Start()
        {
            Log.Information("waiting for auth response...");
            await FetchToken().ConfigureAwait(false);

            Log.Information("starting periodic token refresh...");
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;
            _ = Task.Run(async () => await PeriodicTokenRefresh(token).ConfigureAwait(false), token); // discard to avoid warning CS4014
        }

        protected async Task FetchToken()
        {
            Log.Information("fetch token called...");

            object body;
            if (string.IsNullOrEmpty(_authenticationContext.RefreshToken))
            {
                Log.Information("using apikey and apisecret...");
                body = new { api_key = _apiKey, api_secret = _apiSecret };
            }
            else
            {
                Log.Information("using refresh token...");
                body = new { refresh_token = _authenticationContext.RefreshToken };
            }

            var payload = JsonConvert.SerializeObject(body);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            Log.Information("fetching auth endpoint...");
            HttpResponseMessage response = await _authenticationClient
              .PostAsync("auth", content)
              .ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Log.Information("parsing token...");
                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var obj = JObject.Parse(result);
                _authenticationContext.AccessToken = (string)obj["access_token"];
                _authenticationContext.RefreshToken = (string)obj["refresh_token"];
                _authenticationContext.RefreshInterval = (int)obj["refresh_token_expires_in"] * 1000 / 10;
                Log.Information("AuthenticationContext: {@AuthenticationContext}", _authenticationContext);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (string.IsNullOrEmpty(_authenticationContext.RefreshToken))
                {
                    throw new InvalidCredentialsException("The used credentials (api_key and/or api_secret) are invalid.");
                }
                else
                {
                    Log.Information("refresh token no longer valid.");
                    await RestartPeriodicTokenRefresh().ConfigureAwait(false);
                }
            }
            else
            {
                Log.Warning("unable to fetch auth token.");
            }
        }

        protected async Task RestartPeriodicTokenRefresh()
        {
            try
            {
                await _semaphore.WaitAsync().ConfigureAwait(false);
                Log.Information("restarting periodic token refresh...");
                _cancellationTokenSource.Cancel();
                _authenticationContext = new AuthenticationContext();
                await FetchToken().ConfigureAwait(false);
                _cancellationTokenSource = new CancellationTokenSource();
                var token = _cancellationTokenSource.Token;
                _ = Task.Run(async () => await PeriodicTokenRefresh(token).ConfigureAwait(false), token); // discard to avoid warning CS4014
            }
            finally
            {
                _semaphore.Release();
            }
        }

        protected async Task PeriodicTokenRefresh(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(_authenticationContext.RefreshInterval), cancellationToken).ConfigureAwait(false);
                await FetchToken().ConfigureAwait(false);
            }
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationContext.AccessToken);
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RestartPeriodicTokenRefresh().ConfigureAwait(false);
            }

            return response;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
            base.Dispose(disposing);
        }
    }
}
