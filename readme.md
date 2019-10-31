# fiskaly KassenSichV client for .NET Core

The fiskaly KassenSichV client is an HTTP client that is needed<sup>[1](#fn1)</sup> for accessing the [kassensichv.io](https://kassensichv.io) API that implements a cloud-based, virtual **CTSS** (Certified Technical Security System) / **TSE** (Technische Sicherheitseinrichtung) as defined by the German **KassenSichV** ([Kassen­sich­er­ungsver­ord­nung](https://www.bundesfinanzministerium.de/Content/DE/Downloads/Gesetze/2017-10-06-KassenSichV.pdf)).

Conceptually this client is a thin (convenience) wrapper above the [System.Net.Http.HttpClient](https://www.nuget.org/packages/System.Net.Http) for .NET Core.
This means you will have to look up the [API documentation](https://docs.microsoft.com/en-us/dotnet/api/system.net.http?view=netcore-2.2) of System.Net.Http.HttpClient to learn how this client is used. From a developer's point of view, the only difference is that you have to use the `Fiskaly.Client.ClientFactory` class for the instantiation of a new `HttpClient`.

## Features

- [X] Automatic authentication handling (fetch/refresh JWT and re-authenticate upon 401 errors).
- [X] Automatic retries on failures (server errors or network timeouts/issues).
- [ ] Automatic JSON parsing and serialization of request and response bodies.
- [X] Future: [<a name="fn1">1</a>] compliance regarding [BSI CC-PP-0105-2019](https://www.bsi.bund.de/SharedDocs/Downloads/DE/BSI/Zertifizierung/Reporte/ReportePP/pp0105b_pdf.pdf?__blob=publicationFile&v=7) which mandates a locally executed SMA component for creating signed log messages. 
- [ ] Future: Automatic offline-handling (collection and documentation according to [Anwendungserlass zu § 146a AO](https://www.bundesfinanzministerium.de/Content/DE/Downloads/BMF_Schreiben/Weitere_Steuerthemen/Abgabenordnung/AO-Anwendungserlass/2019-06-17-einfuehrung-paragraf-146a-AO-anwendungserlass-zu-paragraf-146a-AO.pdf?__blob=publicationFile&v=1))

## Install

First of all, you have to initialize the required git submodule(s) using:

```
$ git submodule update --init
```

Then you can build `FiskalyClient` using:

```
$ dotnet build
```

The test suite in `FiskalyClientTest` is run using:

```
$ dotnet test
```

## Usage

```c#
using System;
using System.Net.Http;
using Fiskaly.Client;
using Serilog;
using System.Text;
using System.Threading.Tasks;

namespace Fiskaly.Client
{
    class Demo
    {
        static String ApiKey = Environment.GetEnvironmentVariable("API_KEY"); // create your own API key and secret at https://dashboard.fiskaly.com
        static String ApiSecret = Environment.GetEnvironmentVariable("API_SECRET");
        static HttpClient client;

        public static StringContent Content(string payload)
        {
            return new StringContent(payload, Encoding.UTF8, "application/json");
        }

        public static async Task<String> CreateTss()
        {
            Log.Information("creating tss...");
            var tssGuid = Guid.NewGuid().ToString();
            var url = $"tss/{tssGuid}";
            var payload = $"{{\"description\": \"{tssGuid}\", \"state\": \"INITIALIZED\"}}";

            HttpResponseMessage response = await client
              .PutAsync(url, Content(payload))
              .ConfigureAwait(false);

            String content = await response.Content
              .ReadAsStringAsync()
              .ConfigureAwait(false);

            return content;
        }

        public static async Task<String> ListTss()
        {
            Log.Information("listing tss...");
            
            HttpResponseMessage response = await client
              .GetAsync("tss")
              .ConfigureAwait(false);

            String content = await response.Content
              .ReadAsStringAsync()
              .ConfigureAwait(false);

            return content;
        }

        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
              .WriteTo.Console()
              .CreateLogger();

            client = await ClientFactory.Create(ApiKey, ApiSecret).ConfigureAwait(false);
            var createTssResponse = await CreateTss().ConfigureAwait(false);
            var listTssResponse = await ListTss().ConfigureAwait(false);
            Log.Information("createTssResponse: {Response}", createTssResponse);
            Log.Information("listTssResponse: {Response}", listTssResponse);
        }
    }
}
```

## Related

- [fiskaly.com](https://fiskaly.com)
- [dashboard.fiskaly.com](https://dashboard.fiskaly.com)
- [kassensichv.io](https://kassensichv.io)
- [kassensichv.net](https://kassensichv.net)
