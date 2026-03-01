using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace ManagementPort;

public class ManagementPortBuilder
{
  public static WebApplication CreateManagementWebApplication(ManagementPortOptions options)
  {
    var builder = WebApplication.CreateBuilder(options.WebApplicationArgs);

    string socketPath = GetAbsolutePath(options);

    if (File.Exists(socketPath))
      File.Delete(socketPath);

    builder.WebHost.ConfigureKestrel(serverOptions =>
    {
      serverOptions.ListenUnixSocket(socketPath, listenOptions =>
      {
        listenOptions.Protocols = HttpProtocols.Http2;
      });
    });

    options.ConfigureBuilder(builder);

    var app = builder.Build();

    app.Lifetime.ApplicationStarted.Register(() =>
    {
#pragma warning disable CA1416 // Validate platform compatibility
      File.SetUnixFileMode(socketPath, options.UnixFileMode);
#pragma warning restore CA1416 // Validate platform compatibility
    });

    app.Lifetime.ApplicationStopped.Register(() =>
    {
      File.Delete(socketPath);
    });

    options.ConfigureApp(app);

    return app;
  }

  private static string GetAbsolutePath(ManagementPortOptions options)
  {
    var socketPath = options.SocketPath;

    if (socketPath.StartsWith('~'))
    {
      var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
      socketPath = Path.Combine(home, socketPath.TrimStart('~', '/', '\\'));
    }

    return Path.GetFullPath(socketPath);
  }
}
