using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Veryfi.IntegrationTests
{
    [TestClass]
    public class DocumentsTests
    {
        private static async Task CheckDocumentAndDeleteAsync(
            VeryfiApi api,
            Document document,
            CancellationToken cancellationToken)
        {
            document.Should().NotBeNull();

            var documents = await api.GetDocumentsAsync(
                cancellationToken: cancellationToken);

            documents.Should().NotBeNullOrEmpty();

            var deleteStatus = await api.DeleteDocumentAsync(
                document.Id,
                cancellationToken);

            deleteStatus.Should().NotBeNull();
            deleteStatus.Status.Should().Be("ok");
        }

        [DataTestMethod]
        [DataRow("invoice1.png")]
        [DataRow("receipt.png")]
        [DataRow("receipt_public.jpg")]
        public async Task ProcessUrlTest(string fileName)
        {
            var url = $"https://raw.githubusercontent.com/veryfi/veryfi-csharp/master/src/tests/Veryfi.IntegrationTests/Assets/{fileName}";
            
            await BaseTests.ApiTestAsync(async (api, cancellationToken) =>
            {
                var document = await api.ProcessDocumentAsync(
                    new DocumentUploadOptions
                    {
                        File_url = url,
                    },
                    cancellationToken);

                await CheckDocumentAndDeleteAsync(
                    api,
                    document,
                    cancellationToken);
            });
        }

        [TestMethod]
        public async Task ProcessUrlsTest()
        {
            var urls = new[]
                {
                    "invoice1.png",
                    "receipt.png",
                    "receipt_public.jpg",
                }
                .Select(static fileName =>
                    $"https://raw.githubusercontent.com/veryfi/veryfi-csharp/master/src/tests/Veryfi.IntegrationTests/Assets/{fileName}")
                .ToArray();
                
            await BaseTests.ApiTestAsync(async (api, cancellationToken) =>
            {
                var document = await api.ProcessDocumentAsync(
                    new DocumentUploadOptions
                    {
                        File_urls = urls,
                    },
                    cancellationToken);

                await CheckDocumentAndDeleteAsync(
                    api,
                    document,
                    cancellationToken);
            });
        }

        [DataTestMethod]
        [DataRow("invoice1.png")]
        [DataRow("receipt.png")]
        [DataRow("receipt_public.jpg")]
        public async Task ProcessBase64Test(string fileName)
        {
            var file = new H.Resource(fileName);

            await BaseTests.ApiTestAsync(async (api, cancellationToken) =>
            {
                var document = await api.ProcessDocumentAsync(
                    new DocumentUploadOptions
                    {
                        File_name = file.FileName,
                        File_data = Convert.ToBase64String(file.AsBytes()),
                    },
                    cancellationToken);

                await CheckDocumentAndDeleteAsync(
                    api,
                    document,
                    cancellationToken);
            });
        }

        [DataTestMethod]
        [DataRow("invoice1.png")]
        [DataRow("receipt.png")]
        [DataRow("receipt_public.jpg")]
        public async Task ProcessFileTest(string fileName)
        {
            var file = new H.Resource(fileName);

            await BaseTests.ApiTestAsync(async (api, cancellationToken) =>
            {
                var document = await api.ProcessDocumentFileAsync(
                    file.AsStream(),
                    new DocumentUploadOptions
                    {
                        File_name = file.FileName,
                    },
                    cancellationToken);

                await CheckDocumentAndDeleteAsync(
                    api,
                    document,
                    cancellationToken);
            });
        }
    }
}
