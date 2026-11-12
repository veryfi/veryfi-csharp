using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Veryfi.IntegrationTests
{
    /// <summary>
    /// Checks that the <c>thinking</c> option reaches the request body. No network or credentials needed.
    /// </summary>
    [TestClass]
    public class ThinkingTests
    {
        private const string Url = "https://cdn.example.com/receipt.jpg";

        private sealed class RecordingHandler : HttpMessageHandler
        {
            public JObject Body { get; private set; } = new JObject();

            protected override async Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                var json = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                Body = JObject.Parse(json);
                return new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent("{}", Encoding.UTF8, "application/json"),
                };
            }
        }

        private static (VeryfiApi Api, RecordingHandler Handler) CreateApi()
        {
            var handler = new RecordingHandler();
            var api = new VeryfiApi("username", "apiKey", "clientId", new HttpClient(handler));
            return (api, handler);
        }

        [DataTestMethod]
        [DataRow(DocumentPOSTJSONRequestThinking.Light, "light")]
        [DataRow(DocumentPOSTJSONRequestThinking.Deep, "deep")]
        public async Task ProcessDocumentSendsThinking(DocumentPOSTJSONRequestThinking thinking, string expected)
        {
            var (api, handler) = CreateApi();

            await api.Documents2Async(new DocumentPOSTJSONRequest { File_url = Url, Thinking = thinking });

            handler.Body["thinking"]!.ToString().Should().Be(expected);
        }

        [TestMethod]
        public async Task ProcessDocumentWithoutThinkingDoesNotSendIt()
        {
            var (api, handler) = CreateApi();

            await api.Documents2Async(new DocumentPOSTJSONRequest { File_url = Url });

            handler.Body.ContainsKey("thinking").Should().BeFalse();
        }
    }
}
