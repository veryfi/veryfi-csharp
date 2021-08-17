using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CA1308 // Normalize strings to uppercase

namespace Veryfi
{
    public partial class VeryfiApi
    {
        private string? ClientSecret { get; }

        private async Task SignRequestAsync(
            HttpRequestMessage request,
            string url)
        {
            if (string.IsNullOrWhiteSpace(ClientSecret))
            {
                return;
            }

#if NETSTANDARD2_0_OR_GREATER
            var timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
#else
            var timestamp = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
#endif
            var arguments = new Dictionary<string, string>
            {
                ["timestamp"] = $"{timestamp}",
            };
            switch (request.Method.Method, url)
            {
                case ("POST", "https://api.veryfi.com/api/v7/partner/documents"):
                    switch (request.Content)
                    {
                        case StringContent stringContent:
                        {
                            var json = await stringContent.ReadAsStringAsync().ConfigureAwait(false);
                            var options = JsonConvert.DeserializeObject<DocumentUploadOptions>(json, JsonSerializerSettings);

                            arguments.Add("file_name", options?.File_name ?? string.Empty);
                            arguments.Add("file_data", options?.File_data ?? string.Empty);
                            arguments.Add("categories", string.Join(",", options?.Categories ?? new List<string>()));
                            arguments.Add("auto_delete", $"{options?.Auto_delete ?? 0}");
                            arguments.Add("boost_mode", $"{options?.Boost_mode ?? 0}");
                            arguments.Add("external_id", options?.External_id ?? string.Empty);
                            arguments.Add("file_url", options?.File_url ?? string.Empty);
                            arguments.Add("file_urls", string.Join(",", options?.File_urls ?? new List<string>()));
                            arguments.Add("max_pages_to_process", $"{options?.Max_pages_to_process ?? 0}");
                            arguments.Add("tags", string.Join(",", options?.Tags ?? new List<string>()));
                            arguments.Add("async", $"{options?.Async ?? 0}");

                            break;
                        }
                        case MultipartFormDataContent multipartContent:
                        {
                            foreach (var content in multipartContent
                                .Where(static content => content is StringContent)
                                .Cast<StringContent>())
                            {
                                var key = content.Headers.ContentDisposition.Name;
                                var value = await content.ReadAsStringAsync().ConfigureAwait(false);

                                arguments.Add(key, value);
                            }

                            break;
                        }
                    }

                    break;

                default:
                    var match = Regex.Match(
                        url,
                        @"(https://api.veryfi.com/api/v7/partner/documents/)(?<documentId>\d+)");
                    if (!match.Success)
                    {
                        break;
                    }

                    var documentId = match.Groups["documentId"].Value;
                    arguments.Add("id", documentId);
                    break;
            }
            var signature = GenerateSignature(ClientSecret, arguments);

            request.Headers.Add("X-Veryfi-Request-Timestamp", $"{timestamp}");
            request.Headers.Add("X-Veryfi-Request-Signature", signature);
        }

        internal static string GenerateSignature(
            string clientSecret, 
            Dictionary<string, string> arguments)
        {
            clientSecret = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret));

            var payload = string.Join(
                ",",
                arguments.Select(static pair => $"{pair.Key}:{pair.Value}"));

            var encoding = Encoding.UTF8;
            var secretBytes = encoding.GetBytes(clientSecret);
            var payloadBytes = encoding.GetBytes(payload);

            using var hmac = new HMACSHA256(secretBytes);
            var signature = hmac.ComputeHash(payloadBytes);
            
            return Convert.ToBase64String(signature);
        }
}
}