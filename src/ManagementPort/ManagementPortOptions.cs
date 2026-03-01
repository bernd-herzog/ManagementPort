using Microsoft.AspNetCore.Builder;

namespace ManagementPort;

public class ManagementPortOptions
{
  public string SocketPath { get; set; } = "/var/run/management.sock";

  public UnixFileMode UnixFileMode { get; set; }

  public string[] WebApplicationArgs { get; set; } = [];

  public Action<WebApplicationBuilder> ConfigureBuilder = _ => { };

  public Action<WebApplication> ConfigureApp = _ => { };
}
