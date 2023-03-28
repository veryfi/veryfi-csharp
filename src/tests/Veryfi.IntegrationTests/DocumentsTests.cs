using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Veryfi.IntegrationTests
{
    [TestClass]
    public class DocumentsTests
    {

        private static async Task CheckDocumentAndDeleteAsyncV7(
            VeryfiApi api,
            Int64 documentId,
            CancellationToken cancellationToken)
        {
            var documentsResponse = await api.GetDocumentsAsync(
                cancellationToken: cancellationToken);
            var documents = documentsResponse.As<JArray>();
            documents.Should().NotBeNull();
            documents.Should().NotBeEmpty();

            var deleteStatus = await api.DeleteDocumentAsync(documentId, cancellationToken);
            deleteStatus.Status.Should().Be(OperationStatusStatus.Ok);
        }

        private static async Task CheckDocumentAndDeleteAsync(
            VeryfiApi api,
            Int64 documentId,
            CancellationToken cancellationToken)
        {
            var documentsResponse = await api.DocumentsAsync(
                cancellationToken: cancellationToken);
            var documents = documentsResponse.As<JObject>();
            documents.Should().NotBeNull();
            documents.Should().NotBeEmpty();
            
            var deleteStatus = await api.Documents5Async(
                documentId,
                cancellationToken
            );

            deleteStatus.Status.Should().Be(OperationStatusStatus.Ok);
        }
        
        [TestMethod]
        public async Task GetDocumentsTestV7()
        {
            await BaseTests.ApiTestAsync(async (api, cancellationToken) =>
            {
                var documentsResponse = await api.GetDocumentsAsync(
                    cancellationToken: cancellationToken);
                var documents = documentsResponse.As<JArray>();
                documents.Should().NotBeNull();
                documents.Should().NotBeEmpty();
            });
        }
        
        [DataTestMethod]
        [DataRow("invoice1.png")]
        [DataRow("receipt.png")]
        [DataRow("receipt_public.jpg")]
        public async Task ProcessUrlTestV7(string fileName)
        {
            var url = $"https://raw.githubusercontent.com/veryfi/veryfi-csharp/master/src/tests/Veryfi.IntegrationTests/Assets/{fileName}";
            
            await BaseTests.ApiTestAsync(async (api, cancellationToken) =>
            {
                var request = new DocumentUploadOptionsV7()
                {
                    File_url = url
                };
                var documentResponse = await api.ProcessDocumentAsync(
                    request,
                    cancellationToken);

                var document = documentResponse.As<JObject>();
                document.Should().NotBeNull();
                document.Should().NotBeEmpty();
                var documentId = (long) document["id"]!;
        
                await CheckDocumentAndDeleteAsyncV7(
                    api,
                    documentId,
                    cancellationToken);
            });
        }
        
        [TestMethod]
        public async Task ProcessUrlsTestV7()
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
                var request = new DocumentUploadOptionsV7()
                {
                    File_urls = urls
                };
                var documentResponse = await api.ProcessDocumentAsync(
                    request,
                    cancellationToken);
                
                var document = documentResponse.As<JObject>();
                document.Should().NotBeNull();
                document.Should().NotBeEmpty();
                var documentId = (long) document["id"]!;
        
                await CheckDocumentAndDeleteAsyncV7(
                    api,
                    documentId,
                    cancellationToken);
            });
        }
        
        [DataTestMethod]
        [DataRow("invoice1.png")]
        [DataRow("receipt.png")]
        [DataRow("receipt_public.jpg")]
        public async Task ProcessBase64TestV7(string fileName)
        {
            var file = new H.Resource(fileName);
        
            await BaseTests.ApiTestAsync(async (api, cancellationToken) =>
            {
                string[] categories = { "Meals", "Fat" };
                var documentResponse = await api.ProcessDocumentAsync(
                    new DocumentUploadOptionsV7()
                    {
                        File_name = file.FileName,
                        File_data = Convert.ToBase64String(file.AsBytes()),
                        Categories = categories
                    },
                    cancellationToken);
                
                var document = documentResponse.As<JObject>();
                document.Should().NotBeNull();
                document.Should().NotBeEmpty();
                var documentId = (long) document["id"]!;
        
                await CheckDocumentAndDeleteAsyncV7(
                    api,
                    documentId,
                    cancellationToken);
            });
        }

        [TestMethod]
        public async Task GetDocumentsTest()
        {
            await BaseTests.ApiTestAsync(async (api, cancellationToken) =>
            {
                var documentsResponse = await api.DocumentsAsync(
                    cancellationToken: cancellationToken);
                var documents = documentsResponse.As<JObject>();
                documents.Should().NotBeNull();
                documents.Should().NotBeEmpty();
            });
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
                var request = new DocumentPOSTJSONRequest
                {
                    File_url = url
                };
                var documentResponse = await api.Documents2Async(
                    request,
                    cancellationToken);

                var document = documentResponse.As<JObject>();
                document.Should().NotBeNull();
                document.Should().NotBeEmpty();
                var documentId = (long) document["id"]!;
        
                await CheckDocumentAndDeleteAsync(
                    api,
                    documentId,
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
                var request = new DocumentPOSTJSONRequest
                {
                    File_urls = urls
                };
                var documentResponse = await api.Documents2Async(
                    request,
                    cancellationToken);
                
                var document = documentResponse.As<JObject>();
                document.Should().NotBeNull();
                document.Should().NotBeEmpty();
                var documentId = (long) document["id"]!;
        
                await CheckDocumentAndDeleteAsync(
                    api,
                    documentId,
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
                string[] categories = { "Meals", "Fat" };
                var documentResponse = await api.Documents2Async(
                    new DocumentPOSTJSONRequest
                    {
                        File_name = file.FileName,
                        File_data = Convert.ToBase64String(file.AsBytes()),
                        Categories = categories
                    },
                    cancellationToken);
                
                var document = documentResponse.As<JObject>();
                document.Should().NotBeNull();
                document.Should().NotBeEmpty();
                var documentId = (long) document["id"]!;
        
                await CheckDocumentAndDeleteAsync(
                    api,
                    documentId,
                    cancellationToken);
            });
        }
    }
}
