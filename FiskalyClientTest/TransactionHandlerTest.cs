using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fiskaly.Client.Test.TransactionHandlerTest
{
    public class TestHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.OK) { RequestMessage = request }, cancellationToken);
        }
    }

    public class TransactionHandlerTest
    {
        private TransactionHandler handler;

        [SetUp]
        public void Setup()
        {
            handler = new TransactionHandler(new TestHandler());
        }

        [Test]
        public async Task Intercept()
        {
            var invoker = new HttpMessageInvoker(handler);
            var tssId = Guid.NewGuid().ToString();
            var txId = Guid.NewGuid().ToString();
            var clientId = Guid.NewGuid().ToString();
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, $"https://kassensichv.fiskaly.com/api/v0/tss/{tssId}/tx/{txId}")
            {
                Content = new StringContent(
                $"{{\"type\": \"OTHER\", \"data\": {{ \"binary\": \"test\" }}, \"state\": \"ACTIVE\", \"client_id\": \"{clientId}\"}}",
                Encoding.UTF8,
                "application/json")
            };
            var httpResponseMessage = await invoker.SendAsync(httpRequestMessage, CancellationToken.None).ConfigureAwait(false);
            Assert.AreEqual(
              $"https://kassensichv.fiskaly.com/api/v0/tss/{tssId}/tx/{txId}/log",
              httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri
            );
        }

        [Test]
        public async Task NoIntercept()
        {
            var invoker = new HttpMessageInvoker(handler);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://kassensichv.fiskaly.com/api/v0/tss");
            var httpResponseMessage = await invoker.SendAsync(httpRequestMessage, CancellationToken.None).ConfigureAwait(false);
            Assert.AreEqual(
              "https://kassensichv.fiskaly.com/api/v0/tss",
              httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri
            );
        }
    }
}
