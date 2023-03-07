using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

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
            switch (request.Content)
            {
                case StringContent stringContent:
                {
                    var jsonString = await stringContent.ReadAsStringAsync().ConfigureAwait(false);
                    var json = JObject.Parse(jsonString);
                    foreach (var item in json)
                    {
                        var key = item.Key;
                        var value = item.Value;
                        if (value == null) continue;
                        string stringValue;
                        switch (key)
                        {
                            case "categories":
                                stringValue = string.Join(",", value);
                                break;
                            case "file_urls":
                                stringValue = string.Join(",", value);
                                break;
                            default:
                                stringValue = (string) value!;
                                break;
                        }
                        arguments.Add(key, stringValue);
                    }
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