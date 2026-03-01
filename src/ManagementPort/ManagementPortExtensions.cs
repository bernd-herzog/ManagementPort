using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace ManagementPort;

/// <summary>
/// Extension methods for running the main application and management port concurrently.
/// </summary>
public static class ManagementPortExtensions
{
  /// <summary>
  /// Starts both the main web application and the management application, then waits for both to shut down.
  /// </summary>
  public static async Task RunWithManagementPortAsync(this WebApplication webApplication, WebApplication managementPort)
  {
    ArgumentNullException.ThrowIfNull(webApplication);
    ArgumentNullException.ThrowIfNull(managementPort);

    await webApplication.StartAsync().ConfigureAwait(false);
    await managementPort.StartAsync().ConfigureAwait(false);

    await webApplication.WaitForShutdownAsync().ConfigureAwait(false);
    await managementPort.WaitForShutdownAsync().ConfigureAwait(false);
  }
}
