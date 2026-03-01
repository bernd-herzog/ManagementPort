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

## Use Cases

### Secure Secret Injection

If you want your secrets to only reside in memory on the machine running the service, but still want to automate startup or even hot-swapping your secrets for key rotation.

```csharp
string? apiKey = null;

mgmtApp.MapPost("/inject-secret", ([FromBody] string secret) => apiKey = secret);

app.Use(async (ctx, next) =>
{
    if (ctx.Request.Headers["X-API-KEY"] != apiKey)
    {
        ctx.Response.StatusCode = 401;
        return;
    }
    await next();
});
```

Inject secret at runtime via socket into a running docker container:

```bash
echo '"my-secret"' | docker exec -i CONTAINER_ID curl -s --http2-prior-knowledge --unix-socket /tmp/admin.sock http://localhost/inject-secret -H 'Content-Type: application/json' --data @-
```
