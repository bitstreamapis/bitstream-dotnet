# Bitstream .NET Library

[![NuGet](https://img.shields.io/nuget/v/Bitstream.Net)](https://www.nuget.org/packages/Bitstream.Net/)

<!-- [![License](https://img.shields.io/github/license/bitstream/bitstream-sdk-dotnet)](https://github.com/bitstream/bitstream-sdk-dotnet/blob/main/LICENSE) -->

**Bitstream** - The analytics and intelligence layer for your APIs.

See [bitstreamapis.com](https://www.bitstreamapis.com) for more information and to sign up for a free account.

## Installation

Install the Bitstream .NET SDK via NuGet:

```bash
dotnet add package Bitstream.Net
```

## Usage

```csharp
using Bitstream.Net;

app.UseBitstream(new Dictionary<string, object>
{
    {"ServiceId", "SERVICE_ID"},
    {"LogLevel", 50}
});

```

`SERVICE_ID` is available from your [Bitstream Dashboard](https://app.bitstreamapis.com/). Go to the `Settings` tab when viewing a service and copy the `Service ID`.

Exampe usage in an ASP.NET application:

_`Startup.cs`_

```csharp
namespace test_api;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Bitstream.Net;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseBitstream(new Dictionary<string, object>
        {
            {"ServiceId", "SERVICE_ID"},
            {"LogLevel", 50}
        });

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}

```

## API

#### `UseBitstream(options)`

- `options` - Options object
  - `serviceId` - Service ID
  - `logLevel` - The default is `20`, for full debugging use `50`.
  - `maskFields` - Array of fields to mask in the request and response.
  - `excludeEndpoints` - Array of endpoints to exclude from logging.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
