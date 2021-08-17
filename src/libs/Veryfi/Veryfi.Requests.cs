using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CA1801 // Review unused parameters
// ReSharper disable UnusedParameter.Local

namespace Veryfi
{
    public partial class VeryfiApi
    {
        private static Task PrepareRequestAsync(
            HttpClient client,
            HttpRequestMessage request,
            StringBuilder urlBuilder)
        {
            return Task.FromResult(false);
        }

        private static Task ProcessResponseAsync(
            HttpClient client,
            HttpResponseMessage response,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }

        private async Task PrepareRequestAsync(
            HttpClient client,
            HttpRequestMessage request,
            string url)
        {
            await SignRequestAsync(request, url).ConfigureAwait(false);
        }
    }
}