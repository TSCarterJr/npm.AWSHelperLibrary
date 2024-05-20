
# AwsMetadataHelper

AwsMetadataHelper is a .NET library that provides easy access to AWS EC2 instance metadata and enriches logging with EC2 instance information. This library is designed to help developers retrieve instance metadata and integrate it seamlessly into their logging infrastructure.

## Features

- Retrieve EC2 instance metadata, such as instance ID and instance type.
- Enrich logging messages with EC2 instance information.
- Easy integration with popular logging frameworks.

## Installation

To install the AwsMetadataHelper package, run the following command in the NuGet Package Manager Console:

```sh
Install-Package AwsMetadataHelper
```

Alternatively, you can add it to your project file:

```xml
<PackageReference Include="AwsMetadataHelper" Version="1.0.0" />
```

## Usage

### Retrieving EC2 Instance Metadata

Here's an example of how to use AwsMetadataHelper to retrieve EC2 instance metadata:

```csharp
using System;
using AwsMetadataHelper;

class Program
{
    static async Task Main(string[] args)
    {
        string instanceId = await AwsMetadataHelper.GetInstanceIdAsync();
        Console.WriteLine($"EC2 Instance ID: {instanceId}");
    }
}
```

### Enriching Logging with EC2 Instance Information

To enrich your logging messages with EC2 instance information, follow these steps:

1. **Add the logging extension method in your Startup class:**

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AwsMetadataHelper;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddAwsMetadataLogging();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
```

2. **Configure logging to use AwsMetadataHelper:**

```csharp
using Microsoft.Extensions.Logging;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddAwsMetadataLogger();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
```

## Contributing

Contributions are welcome! Please open an issue or submit a pull request on GitHub.

1. Fork the repository.
2. Create a feature branch.
3. Commit your changes.
4. Push to the branch.
5. Open a pull request.

## Support

If you encounter any issues or have any questions, feel free to open an issue on GitHub or contact us at support@example.com.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
