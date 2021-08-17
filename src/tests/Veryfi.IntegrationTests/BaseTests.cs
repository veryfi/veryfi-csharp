using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Veryfi.IntegrationTests
{
    internal static class BaseTests
    {
        public static async Task ApiTestAsync(Func<VeryfiApi, CancellationToken, Task> action)
        {
            using var source = new CancellationTokenSource(TimeSpan.FromSeconds(120));
            var cancellationToken = source.Token;

            var username = 
                Environment.GetEnvironmentVariable("VERYFI_USERNAME") ??
                throw new AssertInconclusiveException("VERYFI_USERNAME environment variable is not found.");
            var apiKey =
                Environment.GetEnvironmentVariable("VERYFI_API_KEY") ??
                throw new AssertInconclusiveException("VERYFI_API_KEY environment variable is not found.");
            var clientId =
                Environment.GetEnvironmentVariable("VERYFI_CLIENT_ID") ??
                throw new AssertInconclusiveException("VERYFI_CLIENT_ID environment variable is not found.");
            var clientSecret = 
                Environment.GetEnvironmentVariable("VERYFI_CLIENT_SECRET");

            using var client = new HttpClient();
            var api = new VeryfiApi(username, apiKey, clientId, client, clientSecret);

            try
            {
                await action(api, cancellationToken).ConfigureAwait(false);
            }
            catch (ApiException<OperationStatus> exception)
            {
                Console.WriteLine($"Error: {exception.Result.Message ?? exception.Result.Error}");
                throw;
            }
        }
    }
}
