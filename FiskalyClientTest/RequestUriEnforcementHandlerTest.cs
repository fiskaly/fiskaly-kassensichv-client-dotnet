using Fiskaly.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using Serilog;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fiskaly.Client.Test.RequestUriEnforcementHandlerTest
{
    public class TestHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.OK), cancellationToken);
        }
    }

    public class RequestUriEnforcementHandlerTest
    {
        private RequestUriEnforcementHandler handler;

        [SetUp]
        public void Setup()
        {
            handler = new RequestUriEnforcementHandler(new TestHandler());
        }

        [Test]
        public void Prevent()
        {
            var invoker = new HttpMessageInvoker(handler);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://www.orf.at/");
            Assert.ThrowsAsync<InvalidRequestUriException>(
              async () =>
              {
                  await invoker.SendAsync(httpRequestMessage, CancellationToken.None).ConfigureAwait(false);
              }
            );
        }

        [Test]
        public void Allow()
        {
            var invoker = new HttpMessageInvoker(handler);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://kassensichv.fiskaly.com/api/v0/tss");
            Assert.DoesNotThrowAsync(
              async () =>
              {
                  await invoker.SendAsync(httpRequestMessage, CancellationToken.None).ConfigureAwait(false);
              }
            );
        }
    }
}
