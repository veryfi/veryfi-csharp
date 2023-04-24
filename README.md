# Veryfi C# Client Library


[![Language](https://img.shields.io/badge/language-C%23-blue.svg?style=flat-square)](https://github.com/veryfi/veryfi-csharp/search?l=C%23&o=desc&s=&type=Code) 
[![License](https://img.shields.io/github/license/veryfi/veryfi-csharp.svg?label=License&maxAge=86400)](LICENSE) 
[![Requirements](https://img.shields.io/badge/Requirements-.NET%20Standard%202.0-blue.svg)](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard2.0.md) 
[![Requirements](https://img.shields.io/badge/Requirements-.NET%20Framework%204.5-blue.svg)](https://github.com/microsoft/dotnet/tree/master/releases/net45) 
[![Build Status](https://github.com/veryfi/veryfi-csharp/workflows/.NET/badge.svg?branch=master)](https://github.com/veryfi/veryfi-csharp/actions/workflows/dotnet.yml)

**Veryfi** is a C# Client Library for communicating with the [Veryfi OCR API](https://veryfi.com/api/)

## Getting Started

### Obtaining Client ID and user keys
If you don't have an account with Veryfi, please go ahead and 
register here: [https://hub.veryfi.com/signup/api/](https://hub.veryfi.com/signup/api/)

### Documentation
[OpenAPI 3 Specification](https://app.swaggerhub.com/apis/Veryfi/verify-api/)

## Nuget

[![NuGet](https://img.shields.io/nuget/dt/Veryfi.svg?style=flat-square&label=Veryfi)](https://www.nuget.org/packages/Veryfi/)

```
Install-Package Veryfi
```

## Usage

```cs
using Veryfi;

using var client = new HttpClient();
var api = new VeryfiApi("username", "apiKey", "clientId", client);

// Process Base64
var document = await api.ProcessDocumentAsync(
    new DocumentUploadOptions
    {
        File_name = "fileName.jpg",
        File_data = Convert.ToBase64String(bytes),
    });

// Process url
var document = await api.ProcessDocumentAsync(
    new DocumentUploadOptions
    {
        File_url = "https://raw.githubusercontent.com/veryfi/veryfi-csharp/master/src/tests/Veryfi.IntegrationTests/Assets/receipt_public.jpg",
    });

// Process urls
var document = await api.ProcessDocumentAsync(
    new DocumentUploadOptions
    {
        File_urls = new [] {
            "https://raw.githubusercontent.com/veryfi/veryfi-csharp/master/src/tests/Veryfi.IntegrationTests/Assets/receipt_public.jpg",
        },
    });

// Process stream
var document = await api.ProcessDocumentFileAsync(
    new Stream(),
    new DocumentUploadOptions
    {
        File_name = "fileName.jpg",
    });

// Process bytes
var document = await api.ProcessDocumentFileAsync(
    new byte[0],
    new DocumentUploadOptions
    {
        File_name = "fileName.jpg",
    });

// Process path
var document = await api.ProcessDocumentFileAsync(
    "C:/invoice.png",
    new DocumentUploadOptions
    {
        // any custom options, File_name is not required.
    });

//
```

## Live Example

C# .NET Fiddle - https://dotnetfiddle.net/voU3yG  
VB.NET .NET Fiddle - https://dotnetfiddle.net/4B8z6n  

## Developers
The code is generated using https://github.com/RicoSuter/NSwag  
NuGet packages are released and versioned automatically. 
Any commit with the `feat:`/`fix:`/`perf:` prefix will release a new version of the package 
(according to https://www.conventionalcommits.org/en/v1.0.0/).

## Need help?
If you run into any issue or need help installing or using the library, please contact support@veryfi.com.

If you found a bug in this library or would like new features added, then open an issue or pull requests against this repo!

To learn more about Veryfi visit https://www.veryfi.com/
