# ManagementPort

Expose management / monitoring endpoints over a Unix socket.

## What is this?

A .NET library that exposes management or monitoring HTTP endpoints on a Unix socket for health checks, secrets injection, metrics, shutdown, diagnostics, etc. Isolated from both your main API and the network.

## Quick Start

```csharp
using ManagementPort;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/api/hello", () => "Hello from main app!");

var mgmtPort = ManagementPortBuilder.CreateManagementWebApplication(new ManagementPortOptions
{
    SocketPath = "/tmp/admin.sock",
});

mgmtApp.MapHealthChecks("/health");

await app.RunWithManagementPortAsync(mgmtPort);
```
