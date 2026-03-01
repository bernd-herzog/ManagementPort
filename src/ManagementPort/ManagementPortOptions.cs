using Microsoft.AspNetCore.Builder;

namespace ManagementPort;

/// <summary>
/// Configuration options for the management port, including socket path and configuration callbacks.
/// </summary>
public class ManagementPortOptions
{
  /// <summary>
  /// The path to the Unix socket file for the management endpoint.
  /// </summary>
  public string SocketPath { get; set; } = "/var/run/management.sock";

  /// <summary>
  /// The Unix file mode to set on the socket file.
  /// </summary>
  public UnixFileMode UnixFileMode { get; set; }

  /// <summary>
  /// The command-line arguments to pass to the WebApplication when created.
  /// </summary>
  public IReadOnlyList<string> WebApplicationArgs { get; set; } = [];

  /// <summary>
  /// A callback to configure the WebApplicationBuilder before building the application.
  /// </summary>
  public Action<WebApplicationBuilder> ConfigureBuilder { get; set; } = _ => { };

  /// <summary>
  /// A callback to configure the WebApplication after it has been built.
  /// </summary>
  public Action<WebApplication> ConfigureApp { get; set; } = _ => { };
}
