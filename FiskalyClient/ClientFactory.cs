﻿using Serilog;
using System;
using System.Net.Http;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FiskalyClientTest")]
namespace Fiskaly.Client
{
    public static class ClientFactory
    {
        public static HttpClient Create(string apiKey, string apiSecret)
        {
            Log.Information("creating fiskaly client...");
            var authenticationHandler = new AuthenticationHandler(new HttpClientHandler(), apiKey, apiSecret);
            var retryHandler = new RetryHandler(authenticationHandler);
            var transactionHandler = new TransactionHandler(retryHandler);
            var requestUriEnforcementHandler = new RequestUriEnforcementHandler(transactionHandler);
            HttpClient client = new HttpClient(requestUriEnforcementHandler)
            {
                BaseAddress = new Uri(Constants.BaseAddress)
            };
            return client;
        }
    }
}
